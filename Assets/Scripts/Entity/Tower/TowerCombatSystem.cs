using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;

namespace Game.Tower
{   
    public class TowerCombatSystem : ExtendedMonoBehaviour
    {
        public StateMachine State;

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

            bulletLifetime = bulletDataList[index].BulletLifetime;
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
                                bulletDataList[i].Target.GetComponent<Creep.CreepSystem>().GetDamage(towerData.Stats.Damage, towerData);
                                SetTargetReached(bulletDataList[i]);
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

        private void ShotBullet()
        {
            var shotCount = 0;
            var creepList = towerData.RangeSystem.CreepList;

            if (creepList.Count >= 1 + towerData.Stats.MultishotCount)
            {
                shotCount = 1 + towerData.Stats.MultishotCount;              
            }
            else
            {
                shotCount = creepList.Count;
            }
           
            for (int i = 0; i < shotCount; i++)
            {
                CreateBullet(creepList[i]);
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
