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
        private float timer;
        private float bulletLifetime;

        private void Start()
        {         
            towerData = gameObject.GetComponent<Tower>();
            bulletList = new List<GameObject>();
            bulletDataList = new List<Bullet>();

            bulletPool = new ObjectPool
            {
                poolObject = towerData.Bullet
            };

            bulletPool.Initialize();
        }

        public void ShootAtCreep(float delay)
        {
            if (timer < delay)
            {
                timer += 0.5f;

                if (timer == 0.5f)
                {
                    bulletList.Add(bulletPool.GetObject());

                    bulletDataList.Add(bulletList[bulletList.Count - 1].GetComponent<Bullet>());
                    
                    bulletList[bulletList.Count - 1].transform.position = towerData.shootPointTransform.position;
                    bulletList[bulletList.Count - 1].transform.rotation = towerData.movingPartTransform.rotation;

                    bulletLifetime = bulletDataList[bulletDataList.Count - 1].particleSystemList[0].main.startLifetime.constant;

                    bulletList[bulletList.Count - 1].SetActive(true);
                }

                for (int i = 0; i < bulletList.Count; i++)
                {
                    var isDistanceOk = GameManager.CalcDistance(
                        bulletList[i].transform.position, 
                        towerData.rangeCollider.CreepInRangeList[0].transform.position) > towerData.rangeCollider.CreepInRangeList[0].transform.lossyScale.x;

                    if (isDistanceOk)
                    {
                        var position = Vector3.Lerp(bulletList[i].transform.position, towerData.rangeCollider.CreepInRangeList[0].transform.position, Time.deltaTime * 10f);
                        bulletList[i].transform.position = position;
                    }
                    else
                    {
                        bulletDataList[i].DisableParticles();
                    }
                }
            }
            else
            {
                if (bulletList.Count > 0)
                {
                    StartCoroutine(RemoveBullet( bulletLifetime));
                }

                timer = 0;
            }
        }

        public void MoveBulletForward()
        {
            if (bulletList.Count > 0)
            {
                for (int i = 0; i < bulletList.Count; i++)
                {
                    bulletList[i].transform.Translate(Vector3.forward * 200, Space.Self);
                }

                StartCoroutine(RemoveBullet(0.2f));
            }
        }

        public IEnumerator RemoveBullet(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (bulletList.Count > 0)
            {
                bulletList[0].SetActive(false);
                bulletList.RemoveAt(0);
            }
        }
    }
}
