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

        private float bulletSpeed, bulletLifetime, distance, targetScaleX, targetScaleY, attackCooldown;
        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private Transform targetTransform;
        private TowerBaseSystem towerData;
        private Vector3 targetLastPos;
        private ObjectPool bulletPool;
        private int AOEShotRange, chainshotTargetCount, multishotCount;
        private Coroutine bulletCoroutine;
        private bool inCombatState;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            towerData = gameObject.GetComponent<TowerBaseSystem>();
            attackCooldown = towerData.Stats.AttackSpeed;

            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool();
            bulletPool.poolObject = towerData.Bullet;
            bulletPool.parent = transform;          
            bulletPool.Initialize();

            bulletCoroutine = null;

            AOEShotRange = towerData.Stats.AOEShotRange;
            chainshotTargetCount = towerData.Stats.ChainshotTargetsCount;
            multishotCount = towerData.Stats.MultishotCount + 1;

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

        private void CreateBullet()
        {
            bulletList.Add(bulletPool.GetObject());
            var last = bulletList.Count - 1;

            bulletDataList.Add(bulletList[last].GetComponent<BulletSystem>());
            SetBulletData(last);

            bulletList[last].SetActive(true);
        }

        private void SetLastBulletTarget(GameObject target)
        {
            bulletDataList[bulletList.Count - 1].Target = target;
            GetTargetData(bulletList.Count - 1);
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
            targetTransform = bulletDataList[index].Target.transform;
            targetScaleX = targetTransform.GetChild(0).lossyScale.x - 2;
            targetScaleY = targetTransform.GetChild(0).lossyScale.y;
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
                            var offset = new Vector3(Random.Range(-15, 15), targetScaleY + Random.Range(-5, 5), Random.Range(-15, 15));

                            distance = CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.transform.position);
           

                            if (distance > targetScaleX)
                            {
                                bulletList[i].transform.LookAt(bulletDataList[i].Target.transform.position);
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

            if (towerData.RangeSystem.CreepList.Count >= multishotCount)
            {
                shotCount = multishotCount;              
            }
            else
            {
                shotCount = towerData.RangeSystem.CreepList.Count;
            }

            
            for (int i = 0; i < shotCount; i++)
            {
                
                CreateBullet();
                SetLastBulletTarget(towerData.RangeSystem.CreepList[i]);

                Debug.Log(i);
            }

            bulletCoroutine = StartCoroutine(AttackCooldown(attackCooldown));
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
                Debug.Log("enterr combatstate");

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
