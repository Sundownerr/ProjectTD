using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Data.Effects;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
    public class SlowAuraSystem : AuraSystem
    {   
        private new SlowAura effect;
        private float removedAttackSpeed;

        public SlowAuraSystem(SlowAura effect, EntitySystem owner) : base(effect, owner)
        {
            this.effect = effect;        
            this.owner = owner;  
            range.CollideType = CollideWith.Towers;
            range.EntityEntered += OnTowerEnteredRange;         
            range.EntityExit += OnTowerExitRange;   
            range.transform.localScale = new Vector3(effect.Size, 0.001f, effect.Size);
        } 

        private void OnTowerEnteredRange(object sender, EntityEventArgs e)
        {
            var tower = e.Entity as TowerSystem;
            removedAttackSpeed = QoL.GetPercentOfValue(effect.SlowPercent, tower.Stats.AttackSpeed);
            tower.Stats.AttackSpeed -= removedAttackSpeed;
        }

        private void OnTowerExitRange(object sender, EntityEventArgs e)
        {
            var tower = e.Entity as TowerSystem;
            tower.Stats.AttackSpeed += removedAttackSpeed;
        }

        public override void Apply()   
        {
            base.Apply();
        }

        public override void Continue()
        {
            base.Continue();          
        }

        public override void End()
        {         
            base.End();
        }
    }
}