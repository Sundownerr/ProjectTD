using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower.System
{
    public class Special
    {
        public bool IsHaveChainTargets;

        private TowerSystem ownerTower;
       
        public Special(TowerSystem ownerTower)
        {
            this.ownerTower = ownerTower;
        }

        public void Set()
        {
            for (int i = 0; i < ownerTower.GetStats().SpecialList.Length; i++)
                ownerTower.GetStats().SpecialList[i].InitSpecial(ownerTower);
        }

        public int CalculateShotCount()
        {
            var creepList = ownerTower.GetCreepInRangeList();
            var requiredShotCount = 1 + ownerTower.Bullet.GetComponent<BulletSystem>().MultishotCount;

            return creepList.Count >= requiredShotCount ? requiredShotCount : creepList.Count;         
        }

        public void SetChainTarget(BulletSystem bullet)
        {
            var hitTargetList = new Collider[20];
            var layer = 1 << 12;
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, 150, hitTargetList, layer);

            if (hitTargetCount < 1)
                IsHaveChainTargets = false;
            else
            {
                IsHaveChainTargets = true;

                var randomCreep = hitTargetList[Random.Range(0, hitTargetCount)].gameObject;

                bullet.Target = randomCreep;
                bullet.RemainingBounceCount--;
            }       
        }

        public void DamageInAOE(BulletSystem bullet)
        {
            var hitTargetList = new Collider[40];
            var layer = 1 << 12;
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, bullet.AOEShotRange, hitTargetList, layer);

            for (int i = 0; i < hitTargetCount; i++)
                hitTargetList[i].gameObject.GetComponent<Creep.CreepSystem>().GetDamage(ownerTower.GetStats().Damage.Value, ownerTower);
        }

        public void IncreaseStatsPerLevel()
        {
            var specialList = ownerTower.GetStats().SpecialList;

            for (int i = 0; i < specialList.Length; i++)
                specialList[i].IncreaseStatsPerLevel();            
        }
    }
}