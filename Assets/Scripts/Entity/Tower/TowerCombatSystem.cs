using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.Tower
{
    
    public class TowerCombatSystem : ExtendedMonoBehaviour
    {
        public StateMachine State;

        private float bulletSpeed, bulletLifetime, distance, targetScaleX, targetScaleY;
        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private Transform targetTransform;
        private TowerBaseSystem towerData;
        private Vector3 targetLastPos;
        private ObjectPool bulletPool;     
        private bool isCooldown;
        private float attackCooldown;
        private Coroutine bulletCoroutine;
       

        private void Start()
        {         
            towerData = gameObject.GetComponent<TowerBaseSystem>();
            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool
            {
                poolObject = towerData.Bullet,
                parent = transform
            };

            bulletPool.Initialize();
            attackCooldown = towerData.Stats.AttackSpeed;

            State = new StateMachine();
            bulletCoroutine = null;
        }

        private void OnDestroy()
        {
            bulletPool.DestroyPool();
        }

        private IEnumerator CreateBullet(float cooldown)
        {
            
                var isCreepInRange =
                    towerData.RangeSystem.CreepInRangeList.Count > 0 &&
                    towerData.RangeSystem.CreepInRangeList[0] != null;

                if (isCreepInRange)
                {
                    bulletList.Add(bulletPool.GetObject());
                    var last = bulletList.Count - 1;

                    bulletDataList.Add(bulletList[last].GetComponent<BulletSystem>());
                    SetBulletData(last);

                    bulletDataList[last].Target = towerData.RangeSystem.CreepInRangeList[0];
                    GetTargetData(last);

                    bulletList[last].SetActive(true);

                    yield return new WaitForSeconds(cooldown);

                    bulletCoroutine = null;

                    State.ChangeState(new ShootState(this));
                }
            
        }

        private void SetBulletData(int index)
        {
            bulletList[index].transform.position = towerData.ShootPointTransform.position;
            bulletList[index].transform.rotation = towerData.MovingPartTransform.rotation;

            bulletLifetime = bulletDataList[index].BulletLifetime;
            bulletSpeed = bulletDataList[index].Speed;
        }

        private void GetTargetData(int index)
        {
            if (bulletDataList[index].Target != null)
            {
                targetTransform = bulletDataList[index].Target.transform;
                targetScaleX = targetTransform.GetChild(0).lossyScale.x - 2;
                targetScaleY = targetTransform.GetChild(0).lossyScale.y;
            }
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
                    if (targetTransform != null)
                    {
                        var offset = new Vector3(Random.Range(-15, 15), targetScaleY + Random.Range(-5, 5), Random.Range(-15, 15));

                        distance = GM.CalcDistance(bulletList[i].transform.position, targetLastPos);
                        targetLastPos = targetTransform.position + offset;

                        if (!bulletDataList[i].IsReachedTarget)
                        {
                            if (distance > targetScaleX)
                            {
                                bulletList[i].transform.LookAt(targetLastPos);
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

        public void SetStartState()
        {
            if (bulletCoroutine == null)
            {
                State.ChangeState(new ShootState(this));
            }
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
                owner.bulletCoroutine = owner.StartCoroutine(owner.CreateBullet(owner.attackCooldown));
            }

            public void Execute()
            {
                owner.MoveBullet();
            }

            public void Exit()
            {
  
            }
        }

        
    }
}
