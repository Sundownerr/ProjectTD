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

            for (int i = 0; i < tower.TraitSystemList.Count; i++)          
                if (tower.TraitSystemList[i] is BulletTraitSystem trait)    
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

            for (int i = 0; i < tower.Stats.TraitList.Length; i++)            
                if (tower.Stats.TraitList[i] is Multishot multishot)
                {
                    var creepList = tower.CreepInRangeList;
                    var requiredShotCount = 1 + multishot.Count;

                    tower.CombatSystem.ShotCount = 
                        creepList.Count >= requiredShotCount ? requiredShotCount : creepList.Count;
                }                             
        }

        public void OnShooting(object sender, BulletSystem bullet)
        {    
            for (int i = 0; i < tower.Stats.TraitList.Length; i++)  
                if (tower.Stats.TraitList[i] is Chainshot chainshot)              
                    bullet.RemainingBounceCount = chainshot.BounceCount;        
        }

        public void IncreaseStatsPerLevel()
        {
            var traitList = tower.TraitSystemList;

            for (int i = 0; i < traitList.Count; i++)
                traitList[i].IncreaseStatsPerLevel();
        }
    }
}