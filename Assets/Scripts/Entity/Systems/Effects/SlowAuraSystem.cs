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
        private Dictionary<TowerSystem, int> removedAttackSpeedModList;

        public SlowAuraSystem(SlowAura effect, EntitySystem owner) : base(effect, owner)
        {
            this.effect = effect;        
            this.owner = owner;            
            removedAttackSpeedModList = new Dictionary<TowerSystem, int>();
        } 

        private void OnTowerEnteredRange(object sender, EntityEventArgs e)
        {
            if (e.Entity is TowerSystem tower)   
            {        
                var removedAttackSpeedMod = 
                        (int)QoL.GetPercentOfValue(
                            effect.SlowPercent, 
                            tower.Stats.AttackSpeedModifier);             
               
                if (tower.AppliedEffectSystem.CountOf(effect) <= 0)
                    tower.Stats.AttackSpeedModifier -= removedAttackSpeedMod;          
                else                
                    removedAttackSpeedMod = 
                        (int)QoL.GetPercentOfValue(
                            effect.SlowPercent, 
                            tower.Stats.AttackSpeedModifier + effect.SlowPercent);               
                                    
                removedAttackSpeedModList.Add(tower, removedAttackSpeedMod); 
                tower.AppliedEffectSystem.Add(effect); 
            }
        }

        private void OnTowerExitRange(object sender, EntityEventArgs e) => RemoveEffect(e.Entity);    

        private void RemoveEffect(EntitySystem entity)
        {
            if (entity is TowerSystem tower)            
            {               
                if (tower.AppliedEffectSystem.CountOf(effect) <= 1)   
                    if(removedAttackSpeedModList.TryGetValue(tower, out int attackSpeedMod))    
                    {                 
                        tower.Stats.AttackSpeedModifier += attackSpeedMod;                                     
                    }
                removedAttackSpeedModList.Remove(tower);
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