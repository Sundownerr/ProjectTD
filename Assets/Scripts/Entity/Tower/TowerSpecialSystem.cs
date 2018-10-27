using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class TowerSpecialSystem
    {
        public bool IsHaveChainTargets;

        private TowerBaseSystem ownerTower;
       
        public TowerSpecialSystem(TowerBaseSystem ownerTower)
        {
            this.ownerTower = ownerTower;

            for (int i = 0; i < ownerTower.StatsSystem.Stats.SpecialList.Count; i++)
            {
                ownerTower.StatsSystem.Stats.SpecialList[i].InitSpecial(ownerTower);
            }
        }

        public int CalculateShotCount()
        {
            var creepList = ownerTower.RangeSystem.CreepList;
            var requiredShotCount = 1 + ownerTower.Bullet.GetComponent<BulletSystem>().MultishotCount;

            if (creepList.Count >= requiredShotCount)
            {
                return requiredShotCount;
            }
            else
            {
                return creepList.Count;
            }
        }

        public void SetChainTarget(BulletSystem bullet)
        {
            var hitTargetList = new Collider[20];
            var layer = 1 << 12;
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, 150, hitTargetList, layer);

            if (hitTargetCount > 1)
            {
                IsHaveChainTargets = true;

                var randomCreep = hitTargetList[Random.Range(0, hitTargetCount)].gameObject;

                bullet.Target = randomCreep;
                bullet.RemainingBounceCount--;
            }
            else
            {
                IsHaveChainTargets = false;
            }
        }

        public void DamageInAOE(BulletSystem bullet)
        {
            var hitTargetList = new Collider[40];
            var layer = 1 << 12;
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, bullet.AOEShotRange, hitTargetList, layer);

            for (int i = 0; i < hitTargetCount; i++)
            {
                hitTargetList[i].gameObject.GetComponent<Creep.CreepSystem>().GetDamage(ownerTower.StatsSystem.Stats.Damage, ownerTower);
            }
        }
    }
}