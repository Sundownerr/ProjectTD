using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Tower.CombatSystem
{

    public class TowerCombatSystem
    {
        private float attackSpeed;

        public TowerCombatSystem(GameObject tower)
        {
            attackSpeed = tower.GetComponent<Tower>().towerStats.attackSpeed;

        }

        public void ShootAtCreep()
        {
            //    if (timer < delay)
            //    {
            //        timer += 0.5f;

            //        if (timer == 0.5f)
            //        {
            //            bulletList.Add(bulletPool.GetObject());
            //            bulletTransformList.Add(bulletList[bulletList.Count - 1].transform);

            //            bulletList[bulletList.Count - 1].transform.position = towerTransform.position;
            //            bulletList[bulletList.Count - 1].transform.rotation = towerTransform.rotation;

            //            bulletParticleSystemList.Add(bulletList[bulletList.Count - 1].GetComponent<ParticleSystem>());

            //            var em = bulletParticleSystemList[bulletParticleSystemList.Count - 1].emission;
            //            em.enabled = true;

            //            bulletList[bulletList.Count - 1].SetActive(true);


            //        }

            //        for (int i = 0; i < bulletList.Count; i++)
            //        {
            //            if (GameManager.CalcDistance(bulletList[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position) > rangeCollider.CreepInRangeList[0].transform.lossyScale.x)
            //            {
            //                bulletList[i].transform.position = Vector3.Lerp(bulletList[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position, Time.deltaTime * 10f);
            //            }
            //            else
            //            {
            //                var em = bulletParticleSystemList[i].emission;
            //                em.enabled = false;

            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (bulletList.Count > 0)
            //        {
            //            StartCoroutine(RemoveBullets(bulletParticleSystemList[0].main.startLifetime.constant));
            //        }

            //        timer = 0;
            //    }
            //}
        }
    }
}
