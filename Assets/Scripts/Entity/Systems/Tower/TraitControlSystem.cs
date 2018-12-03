using System.Collections.Generic;
using Game.Creep;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class TraitControlSystem : ITowerSystem
    {
        public bool IsHaveChainTargets { get => isHaveChainTargets; set => isHaveChainTargets = value; }

        private bool isHaveChainTargets;
        private TowerSystem tower;
        private Collider[] aoeColliderList, chainColliderList;
        private int creepLayer;      

        public TraitControlSystem(TowerSystem ownerTower) 
        {
            tower = ownerTower;
            aoeColliderList = new Collider[20];
            chainColliderList = new Collider[40];
            creepLayer = 1 << 12;
        } 

        public void Set()
        {
            for (int i = 0; i < tower.Stats.TraitList.Length; i++)
                tower.Stats.TraitList[i].InitTrait(tower);          
        }

        public int CalculateShotCount()
        {
            var creepList = tower.CreepInRangeList;
            var requiredShotCount = 1 + tower.Bullet.GetComponent<BulletSystem>().MultishotCount;

            return creepList.Count >= requiredShotCount ? requiredShotCount : creepList.Count;
        }

        public void SetChainTarget(BulletSystem bullet)
        {       
            var hitTargetCount = Physics.OverlapSphereNonAlloc(
                bullet.transform.position, 
                150, 
                chainColliderList, 
                creepLayer);    

            if (hitTargetCount < 1)
                IsHaveChainTargets = false;
            else
            {
                IsHaveChainTargets = true;
              
                if (bullet.Target != null)
                    for (int i = 0; i < hitTargetCount; i++)                                                    
                        if (bullet.Target.Prefab == chainColliderList[i].gameObject)
                        {
                            bullet.Target = 

                                i - 1 >= 0 ? 
                                    GM.I.CreepList.Find(creep => 
                                        creep.Prefab == chainColliderList[i - 1].transform.gameObject) :

                                i + 1 < hitTargetCount ? 
                                    GM.I.CreepList.Find(creep => 
                                        creep.Prefab == chainColliderList[i + 1].transform.gameObject) :

                                bullet.Target;     
                            break; 
                        }                             
                bullet.RemainingBounceCount--;
            }          
        }

        public void DamageInAOE(BulletSystem bullet)
        {
            var hitTargetCount = Physics.OverlapSphereNonAlloc(
                bullet.transform.position, 
                bullet.AOEShotRange, 
                aoeColliderList, 
                creepLayer);

            for (int i = 0; i < hitTargetCount; i++)
                DamageSystem.DoDamage(
                    GM.I.CreepList.Find(creep => creep.Prefab == aoeColliderList[i].transform.gameObject), 
                    tower.Stats.Damage.Value, 
                    tower);
        }

        public void IncreaseStatsPerLevel()
        {
            var specialList = tower.Stats.TraitList;

            for (int i = 0; i < specialList.Length; i++)
                specialList[i].IncreaseStatsPerLevel();
        }
    }
}