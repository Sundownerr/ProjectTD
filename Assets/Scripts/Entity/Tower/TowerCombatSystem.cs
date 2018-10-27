using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;

namespace Game.Tower
{   
    public class TowerCombatSystem 
    {
        public StateMachine State;
        
        public bool isHaveChainTargets;

        private float bulletSpeed, bulletLifetime;
        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private TowerBaseSystem ownerTower;
        private ObjectPool bulletPool;
        private Coroutine bulletCoroutine;
        private int multishotCount, chainshotCount, AOEShotRange;

        public TowerCombatSystem(TowerBaseSystem ownerTower)
        {          
            this.ownerTower = ownerTower;

            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool();
            bulletPool.poolObject = ownerTower.Bullet;
            bulletPool.parent = ownerTower.transform;          
            bulletPool.Initialize();
            
            bulletCoroutine = null;
            
            State = new StateMachine();                  
        }

        private void OnDestroy()
        {
            bulletPool.DestroyPool();
        }

        public void SetStartState()
        {
            if (bulletCoroutine == null)
            {
                State.ChangeState(new ShootState(this));
            }
        }

        private IEnumerator AttackCooldown(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);

            State.ChangeState(new ShootState(this));
        }

        private void CreateBullet(GameObject target)
        {
            bulletList.Add(bulletPool.GetObject());
            var last = bulletList.Count - 1;

            bulletDataList.Add(bulletList[last].GetComponent<BulletSystem>());
            bulletDataList[bulletList.Count - 1].Target = target;
            SetBulletData(last);
            
            bulletList[last].SetActive(true);
        }

        private void SetBulletData(int index)
        {
            bulletList[index].transform.position = ownerTower.ShootPointTransform.position;
            bulletList[index].transform.rotation = ownerTower.MovingPartTransform.rotation;

            bulletLifetime = bulletDataList[index].Lifetime;
            bulletSpeed = bulletDataList[index].Speed;

        }

   
        private void SetTargetReached(BulletSystem bullet)
        {
            if (!bullet.IsReachedTarget)
            {           
                bullet.IsReachedTarget = true;
                bullet.Show(false);
                ownerTower.StartCoroutine(RemoveBullet(bulletLifetime));
            }          
        }
      
        public void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].activeSelf)
                {
                    if (bulletDataList[i].Target != null)
                    {                                    
                        if (!bulletDataList[i].IsReachedTarget)
                        {
                            var targetTransform = bulletDataList[i].Target.transform;
                            var targetScaleX = targetTransform.GetChild(0).lossyScale.x - 2;
                            var targetScaleY = targetTransform.GetChild(0).lossyScale.y;                           

                            var distance = ExtendedMonoBehaviour.CalcDistance(bulletList[i].transform.position, targetTransform.position);         

                            if (distance > targetScaleX)
                            {
                                var offset = new Vector3(0, targetScaleY / 2, 0);
                                var targetPos = targetTransform.position + offset;

                                bulletList[i].transform.LookAt(targetPos);
                                bulletList[i].transform.Translate(Vector3.forward * bulletSpeed, Space.Self);
                            }
                            else
                            {                               
                                HitTarget(i);
                            }                   
                        }
                    }
                    else
                    {                       
                        bulletList[i].transform.Translate(Vector3.forward * Random.Range(1, bulletSpeed), Space.Self);
                        SetTargetReached(bulletDataList[i]);
                    }
                }
            }
        }

        public bool CheckAllBulletInactive()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].activeSelf)
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerator RemoveBullet(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (bulletList.Count > 0)
            {
                bulletList[0].SetActive(false);
                bulletDataList.RemoveAt(0);
                bulletList.RemoveAt(0);            
            }
        }

        private int CalculateShotCount()
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

        private void SetChainTarget(BulletSystem bulletData, int chainCount)
        {
            var hitTargets = new Collider[20];
            var layer = 1 << 12; 
            var count = Physics.OverlapSphereNonAlloc(bulletData.transform.position, 150, hitTargets, layer);

            if (count > 1)
            {
                isHaveChainTargets = true;

                var randomCreep = hitTargets[Random.Range(0, count)].gameObject;

                if (bulletData.Target != randomCreep)
                {
                    bulletData.Target = randomCreep;
                    bulletData.RemainingBounceCount--;
                }
            }
            else
            {
                isHaveChainTargets = false;
            }
        }

        private void DamageInAOE(GameObject bullet, int AOE)
        {
            var hitTargets = new Collider[40];
            var layer = 1 << 12;
            var count = Physics.OverlapSphereNonAlloc(bullet.transform.position, AOE, hitTargets, layer);

            for (int i = 0; i < count; i++)
            {
                hitTargets[i].gameObject.GetComponent<Creep.CreepSystem>().GetDamage(ownerTower.StatsSystem.Stats.Damage, ownerTower);
            }
        }

        private void HitTarget(int bulletIndex)
        {
            if (bulletDataList[bulletIndex].AOEShotRange > 0)
            {
                DamageInAOE(bulletList[bulletIndex], bulletDataList[bulletIndex].AOEShotRange);
            }
            else
            {
                bulletDataList[bulletIndex].Target.GetComponent<Creep.CreepSystem>().GetDamage(ownerTower.StatsSystem.Stats.Damage, ownerTower);
            }

            var isChainShot =
                bulletDataList[bulletIndex].ChainshotCount > 0 &&
                bulletDataList[bulletIndex].RemainingBounceCount > 0;

            if (isChainShot)
            {
                SetChainTarget(bulletDataList[bulletIndex], bulletDataList[bulletIndex].ChainshotCount);
            }
            else
            {
                SetTargetReached(bulletDataList[bulletIndex]);
            }               
        }

        private void ShotBullet()
        {
            var shotCount = CalculateShotCount();
                    
            for (int i = 0; i < shotCount; i++)
            {
                CreateBullet(ownerTower.RangeSystem.CreepList[i]);
            }

            bulletCoroutine = ownerTower.StartCoroutine(AttackCooldown(ownerTower.StatsSystem.Stats.AttackSpeed));
        }
      
        protected class ShootState : IState
        {
            private TowerCombatSystem owner;

            public ShootState(TowerCombatSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                var isCreepInRange =
                   owner.ownerTower.RangeSystem.CreepList.Count > 0 &&
                   owner.ownerTower.RangeSystem.CreepList[0] != null;

                if (isCreepInRange)
                {
                    owner.ShotBullet();
                }
            }

            public void Execute()
            {
                owner.MoveBullet();
            }

            public void Exit()
            {
                owner.bulletCoroutine = null;
            }
        }       
    }
}
