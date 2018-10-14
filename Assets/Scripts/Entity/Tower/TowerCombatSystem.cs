using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.Tower
{
    
    public class TowerCombatSystem : ExtendedMonoBehaviour
    {
        public bool IsAllBulletsInactive;

        private float bulletSpeed, bulletLifetime, distance, targetScaleX, targetScaleY;
        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private Transform targetTransform;
        private TowerBaseSystem towerData;
        private Vector3 targetLastPos;
        private ObjectPool bulletPool;     
        private bool isCooldown;          

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
            IsAllBulletsInactive = true;
        }

        private void OnDestroy()
        {
            bulletPool.DestroyPool();
        }

        private IEnumerator CreateBullet(float cooldown)
        {          
            bulletList.Add(bulletPool.GetObject());
            var last = bulletList.Count - 1;

            bulletDataList.Add(bulletList[last].GetComponent<BulletSystem>());

            bulletList[last].transform.position = towerData.ShootPointTransform.position;
            bulletList[last].transform.rotation = towerData.MovingPartTransform.rotation;

            bulletLifetime = bulletDataList[last].BulletLifetime;
            bulletSpeed = bulletDataList[last].Speed;
            
            bulletDataList[last].Target = towerData.RangeSystem.CreepInRangeList[0];        

            GetTargetData(last);

            bulletList[last].SetActive(true);

            yield return new WaitForSeconds(cooldown);

            isCooldown = false;
        }

        private void GetTargetData(int index)
        {
            targetTransform = bulletDataList[index].Target.transform;          
            targetScaleX = targetTransform.GetChild(0).lossyScale.x - 2;
            targetScaleY = targetTransform.GetChild(0).lossyScale.y;
        }

        private void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (targetTransform != null)
                {
                    distance = GM.CalcDistance(bulletList[i].transform.position, targetLastPos);
                    targetLastPos = targetTransform.position + new Vector3(Random.Range(-15, 15), targetScaleY + Random.Range(-5, 5), Random.Range(-15, 15));
                }
                else
                {
                    distance = 0;                    
                    bulletList[i].transform.Translate(Vector3.forward * Random.Range(1, bulletSpeed), Space.Self);
                }

                if (!bulletDataList[i].IsReachedTarget && distance > targetScaleX)
                {
                    bulletList[i].transform.LookAt(targetLastPos);
                    bulletList[i].transform.Translate(Vector3.forward * bulletSpeed, Space.Self);
                }
                else
                {
                    if (!bulletDataList[i].IsReachedTarget)
                    {
                        bulletDataList[i].IsReachedTarget = true;
                        bulletDataList[i].Show(false);

                        StartCoroutine(RemoveBullet(bulletLifetime));

                        if (bulletDataList[i].Target != null)
                        {
                            bulletDataList[i].Target.GetComponent<Creep.CreepSystem>().GetDamage(towerData.Stats.Damage, towerData);
                        }
                    }
                }
            }
        }

        public void Shoot(float cooldown)
        {
            if(!isCooldown)
            {
                isCooldown = true;
                StartCoroutine(CreateBullet(cooldown));
            }

            MoveBullet();
        }

        public bool CheckAllBulletInactive()
        {
            bool response = true;

            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].activeSelf)
                {
                    response = false;
                    break;
                }
            }

            IsAllBulletsInactive = response;
            return response;
        }

        public void MoveBulletOutOfRange()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].activeSelf)
                {
                    MoveBullet();
                }
            }
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

            CheckAllBulletInactive();
        }
    }
}
