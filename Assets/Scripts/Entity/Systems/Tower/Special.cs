using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class Special : ITowerSystem
    {
        public bool IsHaveChainTargets { get => isHaveChainTargets; set => isHaveChainTargets = value; }

        private bool isHaveChainTargets;
        private TowerSystem tower;
        private Collider[] aoeColliderList, chainShotColliderList;
        private int creepLayer;      

        public Special(TowerSystem ownerTower) 
        {
            tower = ownerTower;
            aoeColliderList = new Collider[20];
            chainShotColliderList = new Collider[40];
            creepLayer = 1 << 12;
        } 

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
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, 150, chainShotColliderList, creepLayer); 

            if (hitTargetCount < 1)
                IsHaveChainTargets = false;
            else
            {
                IsHaveChainTargets = true;
              
                if (bullet.Target != null)
                    for (int i = 0; i < hitTargetCount; i++)                                                    
                        if (bullet.Target.Prefab == chainShotColliderList[i].gameObject)
                        {
                            bullet.Target = 
                                i - 1 >= 0              ? GM.I.CreepSystemList.Find(creep => creep.Prefab == chainShotColliderList[i - 1].transform.gameObject) :
                                i + 1 < hitTargetCount  ? GM.I.CreepSystemList.Find(creep => creep.Prefab == chainShotColliderList[i + 1].transform.gameObject) :
                                bullet.Target;     
                            break; 
                        }      
                              
                bullet.RemainingBounceCount--;
            }          
        }

        public void DamageInAOE(BulletSystem bullet)
        {
            var hitTargetCount = Physics.OverlapSphereNonAlloc(bullet.transform.position, bullet.AOEShotRange, aoeColliderList, creepLayer);

            for (int i = 0; i < hitTargetCount; i++)
                DamageSystem.DoDamage(GM.I.CreepSystemList.Find(creep => creep.Prefab == aoeColliderList[i].transform.gameObject), tower.Stats.Damage.Value, tower);
        }

        public void IncreaseStatsPerLevel()
        {
            var specialList = tower.Stats.SpecialList;

            for (int i = 0; i < specialList.Length; i++)
                specialList[i].IncreaseStatsPerLevel();
        }
    }
}