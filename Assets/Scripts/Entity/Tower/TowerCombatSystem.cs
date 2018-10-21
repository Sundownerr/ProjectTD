using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;

namespace Game.Tower
{   
    public class TowerCombatSystem : ExtendedMonoBehaviour
    {
        public StateMachine State;
        public LayerMask CreepLayer;
        public bool isHaveChainTargets;

        private float bulletSpeed, bulletLifetime;
        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
   
        private TowerBaseSystem towerData;
        private ObjectPool bulletPool;
        private Coroutine bulletCoroutine;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            towerData = GetComponent<TowerBaseSystem>();

            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool();
            bulletPool.poolObject = towerData.Bullet;
            bulletPool.parent = transform;          
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
            bulletList[index].transform.position = towerData.ShootPointTransform.position;
            bulletList[index].transform.rotation = towerData.MovingPartTransform.rotation;
            bulletDataList[index].chainCount = towerData.Stats.ChainshotCount;
            bulletLifetime = bulletDataList[index].Lifetime;
            bulletSpeed = bulletDataList[index].Speed;
        }

        private void SetTargetReached(BulletSystem bullet)
        {
            if (!bullet.IsReachedTarget)
            {           
                bullet.IsReachedTarget = true;
                bullet.Show(false);
                StartCoroutine(RemoveBullet(bulletLifetime));
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

                            var distance = CalcDistance(bulletList[i].transform.position, targetTransform.position);         

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
            var creepList = towerData.RangeSystem.CreepList;

            if (creepList.Count >= 1 + towerData.Stats.MultishotCount)
            {
                return 1 + towerData.Stats.MultishotCount;
            }
            else
            {
                return creepList.Count;
            }
        }

        private void SetChainTarget(BulletSystem bulletData, int chainCount)
        {
            var hitTargets = new Collider[20];
            var count = Physics.OverlapSphereNonAlloc(bulletData.transform.position, 150, hitTargets, CreepLayer);

            if (count > 1)
            {
                isHaveChainTargets = true;

                var randomCreep = hitTargets[Random.Range(0, count)].gameObject;

                if (bulletData.Target != randomCreep)
                {
                    bulletData.Target = randomCreep;
                    bulletData.chainCount--;
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
            var count = Physics.OverlapSphereNonAlloc(bullet.transform.position, AOE, hitTargets, CreepLayer);

            for (int i = 0; i < count; i++)
            {
                hitTargets[i].gameObject.GetComponent<Creep.CreepSystem>().GetDamage(towerData.Stats.Damage, towerData);
            }
        }

        private void HitTarget(int bulletIndex)
        {
            if (towerData.Stats.AOEShotRange > 0)
            {
                DamageInAOE(bulletList[bulletIndex], towerData.Stats.AOEShotRange);
            }
            else
            {
                bulletDataList[bulletIndex].Target.GetComponent<Creep.CreepSystem>().GetDamage(towerData.Stats.Damage, towerData);
            }

            var isChainShot =
                towerData.Stats.ChainshotCount > 0 &&
                bulletDataList[bulletIndex].chainCount > 0;

            if (isChainShot)
            {
                SetChainTarget(bulletDataList[bulletIndex], towerData.Stats.ChainshotCount);
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
                CreateBullet(towerData.RangeSystem.CreepList[i]);
            }

            bulletCoroutine = StartCoroutine(AttackCooldown(towerData.Stats.AttackSpeed));
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
                   owner.towerData.RangeSystem.CreepList.Count > 0 &&
                   owner.towerData.RangeSystem.CreepList[0] != null;

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
