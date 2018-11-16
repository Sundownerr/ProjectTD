using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class Special
    {
        public bool IsHaveChainTargets { get => isHaveChainTargets; set => isHaveChainTargets = value; }

        private bool isHaveChainTargets;
        private TowerSystem tower;

        public Special(TowerSystem ownerTower) => tower = ownerTower;

        public void Set()
        {
            for (int i = 0; i < tower.Stats.SpecialList.Length; i++)
                tower.Stats.SpecialList[i].InitSpecial(tower);
        }

        public int CalculateShotCount()
        {
            var creepList = tower.CreepInRangeList;
            var requiredShotCount = 1 + tower.Bullet.GetComponent<BulletSystem>().MultishotCount;

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
                var isNewTargetFound = false;

                for (int i = 0; i < hitTargetCount; i++)
                {
                    var randomCreep = hitTargetList[UnityEngine.Random.Range(0, hitTargetCount)].gameObject;
                    if (bullet.Target != randomCreep)
                    {
                        isNewTargetFound = true;
                        bullet.Target = randomCreep;
                        break;
                    }
                }

                if (!isNewTargetFound)
                    bullet.Target = null;

                bullet.RemainingBounceCount--;
            }
        }

        public void DamageInAOE(BulletSystem bullet)
        {
            var hitTargetList = new Collider[40];
            var layer = 1 << 12;
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, bullet.AOEShotRange, hitTargetList, layer);

            for (int i = 0; i < hitTargetCount; i++)
                DamageSystem.DoDamage(hitTargetList[i].gameObject.GetComponent<Creep.CreepSystem>(), tower.Stats.Damage.Value, tower);
        }

        public void IncreaseStatsPerLevel()
        {
            var specialList = tower.Stats.SpecialList;

            for (int i = 0; i < specialList.Length; i++)
                specialList[i].IncreaseStatsPerLevel();
        }
    }
}