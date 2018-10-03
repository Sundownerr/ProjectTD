using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.Tower
{
    
    public class TowerCombatSystem : ExtendedMonoBehaviour
    {
        public TowerBaseSystem towerData;
        public ObjectPool bulletPool;

        private List<GameObject> bulletList;
        private List<BulletSystem> bulletDataList;       
        private float bulletSpeed, bulletLifetime, distance, targetScale;
        private bool isCooldown;
        private Vector3 targetLastPos;
        private Transform targetTransform;
        private GameObject target, newTarget;

        private void Start()
        {         
            towerData = gameObject.GetComponent<TowerBaseSystem>();
            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool
            {
                poolObject = towerData.Bullet,
                parent = null
            };

            bulletPool.Initialize();
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
            bulletDataList[bulletDataList.Count - 1].Target = towerData.RangeSystem.CreepInRangeList[0];
            target = towerData.RangeSystem.CreepInRangeList[0];

            bulletList[last].SetActive(true);

            yield return new WaitForSeconds(cooldown);

            isCooldown = false;

            if (bulletList.Count > 0)
            {
                StartCoroutine(RemoveBullet(bulletLifetime - 0.1f));
            }
        }

        private void GetTargetData(int index)
        {
            targetTransform = bulletDataList[index].Target.transform;

            targetLastPos = targetTransform.position + new Vector3(0, targetTransform.lossyScale.y / 2, 0);

            targetScale = targetTransform.lossyScale.x - 2;

            distance = GameManager.CalcDistance(bulletList[index].transform.position, targetLastPos);
        }

        private void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletDataList[i].Target != null)
                {
                    GetTargetData(i);
                }

                if (!bulletDataList[i].IsReachedTarget && distance > targetScale)
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

                        if (bulletDataList[i].Target != null)
                        {
                            bulletDataList[i].Target.GetComponent<Creep.CreepSystem>().GetDamage(towerData.TowerStats.Damage);
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

        public void MoveBulletOutOfRange()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if(bulletList[i].activeSelf)
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
        }
    }
}
