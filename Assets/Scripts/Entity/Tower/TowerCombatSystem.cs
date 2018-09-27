using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;

namespace Game.Tower.CombatSystem
{

    public class TowerCombatSystem : MonoBehaviour
    {
        public Tower towerData;
     
        private List<GameObject> bulletList;
        private List<Bullet> bulletDataList;
        private ObjectPool bulletPool;
        private float bulletSpeed;
        private float bulletLifetime;
        private bool isCooldown;
        

        private void Start()
        {         
            towerData = gameObject.GetComponent<Tower>();
            bulletList = new List<GameObject>();
            bulletDataList = new List<Bullet>();          
        }

        private IEnumerator CreateBullet(float cooldown)
        {
            if (bulletPool == null)
            {
                bulletPool = new ObjectPool
                {
                    poolObject = towerData.Bullet
                };

                bulletPool.Initialize();
            }

            bulletList.Add(bulletPool.GetObject());
            var last = bulletList.Count - 1;

            bulletDataList.Add(bulletList[last].GetComponent<Bullet>());

            bulletList[last].transform.position = towerData.shootPointTransform.position;
            bulletList[last].transform.rotation = towerData.movingPartTransform.rotation;

            bulletLifetime = bulletDataList[last].BulletLifetime;
            bulletSpeed = bulletDataList[last].Speed;
            bulletDataList[last].Target = towerData.rangeCollider.CreepInRangeList[0];

            bulletList[last].SetActive(true);

            yield return new WaitForSeconds(cooldown);

            isCooldown = false;

            if (bulletList.Count > 0)
            {
                StartCoroutine(RemoveBullet(bulletLifetime));
            }
        }

        public void ShootAtCreep(float cooldown)
        {
            if(!isCooldown)
            {
                isCooldown = true;
                StartCoroutine(CreateBullet(cooldown));
            }              

            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletDataList[i].Target != null)
                {
                    var distance = GameManager.CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.transform.position);

                    if (!bulletDataList[i].IsDestinationReached && distance > bulletDataList[i].Target.transform.lossyScale.x)
                    {
                        bulletList[i].transform.LookAt(bulletDataList[i].Target.transform);
                        bulletList[i].transform.Translate(Vector3.forward * bulletSpeed, Space.Self);
                    }
                    else
                    {
                        bulletDataList[i].IsDestinationReached = true;
                        bulletDataList[i].DisableParticles();
                    }
                }                           
            }
        }

        public void MoveBulletOutOfRange()
        {
            if (bulletList.Count > 0)
            {

                for (int i = 0; i < bulletList.Count; i++)
                {
                    if (bulletDataList[i].Target != null)
                    {
                        var distance = GameManager.CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.transform.position);

                        bulletList[i].transform.LookAt(bulletDataList[i].Target.transform);
                        bulletList[i].transform.Translate(Vector3.forward * (25f + distance / 10), Space.Self);
                    }
                }
                StartCoroutine(RemoveBullet(bulletLifetime));
            }

        }


        public IEnumerator RemoveBullet(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (bulletList.Count > 0)
            {
                bulletList[0].SetActive(false);
                bulletDataList[0].DisableParticles();
                bulletDataList.RemoveAt(0);
                bulletList.RemoveAt(0);
            }
        }
    }
}
