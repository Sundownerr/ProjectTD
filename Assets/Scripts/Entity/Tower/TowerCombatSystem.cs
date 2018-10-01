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

        private void Start()
        {         
            towerData = gameObject.GetComponent<TowerBaseSystem>();
            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool
            {
                poolObject = towerData.Bullet
            };

            bulletPool.Initialize();
        }

        private IEnumerator CreateBullet(float cooldown)
        {          
            bulletList.Add(bulletPool.GetObject());
            var last = bulletList.Count - 1;

            bulletDataList.Add(bulletList[last].GetComponent<BulletSystem>());

            bulletList[last].transform.position = towerData.shootPointTransform.position;
            bulletList[last].transform.rotation = towerData.movingPartTransform.rotation;

            bulletLifetime = bulletDataList[last].BulletLifetime;
            bulletSpeed = bulletDataList[last].Speed;             

            bulletList[last].SetActive(true);

            yield return new WaitForSeconds(cooldown);

            isCooldown = false;

            if (bulletList.Count > 0)
            {
                StartCoroutine(RemoveBullet(bulletLifetime));
            }
        }

        private void MoveBullet()
        {

            for (int i = 0; i < bulletList.Count; i++)
            {
                if (towerData.TowerRange.CreepInRangeList.Count > 0)
                {
                    bulletDataList[i].Target = towerData.TowerRange.CreepInRangeList[0];
                }

                if (bulletDataList[i].Target != null)
                {
                    targetTransform = bulletDataList[i].Target.transform;

                    targetLastPos = targetTransform.position + new Vector3(0, targetTransform.lossyScale.y/ 2, 0);

                    targetScale = targetTransform.lossyScale.x - 2;

                    distance = GameManager.CalcDistance(
                            bulletList[i].transform.position,
                            targetLastPos
                            );               
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

        public void ShootAtCreep(float cooldown)
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
            if (bulletList.Count > 0)
            {
                MoveBullet();

                StartCoroutine(RemoveBullet(bulletLifetime));
            }
        }

        public IEnumerator RemoveBullet(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (bulletList.Count > 0)
            {
                bulletList[0].SetActive(false);
                bulletDataList[0].Show(false);
                bulletDataList.RemoveAt(0);
                bulletList.RemoveAt(0);
            }
        }
    }
}
