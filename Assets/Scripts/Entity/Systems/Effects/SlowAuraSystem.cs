using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Data;
using Game.Data.Effects;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
    public class SlowAuraSystem : AuraSystem
    {   
        private new SlowAura effect;
        private Dictionary<TowerSystem, float> removedAttackSpeedList;

        public SlowAuraSystem(SlowAura effect, EntitySystem owner) : base(effect, owner)
        {
            this.effect = effect;        
            this.owner = owner;            
            removedAttackSpeedList = new Dictionary<TowerSystem, float>();
        } 

        private void OnTowerEnteredRange(object sender, EntityEventArgs e)
        {
            if (e.Entity is TowerSystem tower)                   
                if (tower.AppliedEffectSystem.CountOf(effect) <= 0)
                {     
                    var removedAttackSpeed = QoL.GetPercentOfValue(effect.SlowPercent, tower.Stats.AttackSpeed);                    
                    removedAttackSpeedList.Add(tower, removedAttackSpeed);
                    tower.Stats.AttackSpeed += removedAttackSpeed;
                    tower.AppliedEffectSystem.Add(effect); 
                }              
        }

        private void OnTowerExitRange(object sender, EntityEventArgs e) => RemoveEffect(e.Entity);    

        private void RemoveEffect(EntitySystem entity)
        {
            if (entity is TowerSystem tower)            
            {               
                if (tower.AppliedEffectSystem.CountOf(effect) == 1)                        
                    tower.Stats.AttackSpeed -= removedAttackSpeedList[tower];                                               
                tower.AppliedEffectSystem.Remove(effect);
            }
        }

        public override void Apply()   
        {
            base.Apply();

            (owner as CreepSystem).HealthSystem.CreepDied += OnOwnerKilled;          
            range.EntityEntered += OnTowerEnteredRange;         
            range.EntityExit += OnTowerExitRange;   

            range.CollideType = CollideWith.Towers;
            range.transform.localScale = new Vector3(effect.Size, 0.001f, effect.Size);
            range.transform.position += new Vector3(0, 15, 0);
            range.SetShow(true);         
        }

        private void OnOwnerKilled(object sender, CreepData creep) => End();   

        public override void Continue()
        {
            base.Continue();         
        }         
        
        public override void End()
        {                             
            for (int i = 0; i < range.EntitySystemList.Count; i++)       
                RemoveEffect(range.EntitySystemList[i]);            
            
            range.EntityEntered -= OnTowerEnteredRange;         
            range.EntityExit -= OnTowerExitRange;   
            base.End();
        }
    }
}