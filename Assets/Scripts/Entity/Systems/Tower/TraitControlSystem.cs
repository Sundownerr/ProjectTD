using System;
using System.Collections.Generic;
using Game.Creep;
using Game.Data;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class TraitControlSystem : ITowerSystem
    {
        public bool IsHaveChainTargets { get => isHaveChainTargets; set => isHaveChainTargets = value; }

        private bool isHaveChainTargets;
        private TowerSystem tower;

        public TraitControlSystem(TowerSystem ownerTower) 
        {
            tower = ownerTower;
            
        } 

        public void Set()
        {
            tower.CombatSystem.BulletHit += OnBulletHit;
            tower.CombatSystem.PrepareToShoot += OnPrepareToShoot;
            tower.CombatSystem.Shooting += OnShooting;
        }

        public void OnBulletHit(object sender, BulletSystem bullet)
        {
            var bulletTraitCount = 0;
            var isHaveChainShot = false;

            for (int i = 0; i < tower.TraitSystems.Count; i++)          
                if (tower.TraitSystems[i] is IBulletTraitSystem trait)    
                {
                    bulletTraitCount++;
                    trait.Apply(bullet); 

                    if (trait is ChainshotSystem)
                        isHaveChainShot = true;
                }
                    
            if (bulletTraitCount == 0)
            {
                if (bullet.Target != null)
                    DamageSystem.DoDamage(bullet.Target, tower.Stats.Damage.Value, tower);

                tower.CombatSystem.SetTargetReached(bullet);
            }           
            else
            {
                if (!isHaveChainShot)              
                    tower.CombatSystem.SetTargetReached(bullet);             
            }            
        }

        public void OnPrepareToShoot(object sender, EventArgs e)
        {
            tower.CombatSystem.ShotCount = 1;

            for (int i = 0; i < tower.Stats.Traits.Count; i++)            
                if (tower.Stats.Traits[i] is Multishot multishot)
                {
                    var creeps = tower.CreepsInRange;
                    var requiredShotCount = 1 + multishot.Count;

                    tower.CombatSystem.ShotCount = 
                        creeps.Count >= requiredShotCount ? requiredShotCount : creeps.Count;
                }                             
        }

        public void OnShooting(object sender, BulletSystem bullet)
        {    
            for (int i = 0; i < tower.Stats.Traits.Count; i++)  
                if (tower.Stats.Traits[i] is Chainshot chainshot)              
                    bullet.RemainingBounceCount = chainshot.BounceCount;        
        }

        public void IncreaseStatsPerLevel()
        {
            for (int i = 0; i < tower.TraitSystems.Count; i++)
                tower.TraitSystems[i].IncreaseStatsPerLevel();
        }
    }
}