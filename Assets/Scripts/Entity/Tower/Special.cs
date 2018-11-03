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
            for (int i = 0; i < tower.GetStats().SpecialList.Length; i++)
                tower.GetStats().SpecialList[i].InitSpecial(tower);
        }

        public int CalculateShotCount()
        {
            var creepList = tower.GetCreepInRangeList();
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
                hitTargetList[i].gameObject.GetComponent<Creep.CreepSystem>().GetDamage(tower.GetStats().Damage.Value, tower);
        }

        public void IncreaseStatsPerLevel()
        {
            var specialList = tower.GetStats().SpecialList;

            for (int i = 0; i < specialList.Length; i++)
                specialList[i].IncreaseStatsPerLevel();            
        }
    }
}