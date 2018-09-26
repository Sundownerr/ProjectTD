using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;
using Game.Tower.CombatSystem;

namespace Game.Tower
{

    public class Tower : ExtendedMonoBehaviour
    {
        public GameObject Bullet;
        public bool IsTowerBuilded;
        public TowerStats towerStats;
        public Transform towerRangeTransform, movingPartTransform, shootPointTransform;
        public RangeCollider rangeCollider;

        private List<Renderer> towerRendererList;
        public TowerCombatSystem TowerCombatSystem;

        private void StartTowerBuild()
        {
            for (int i = 0; i < towerRendererList.Count; i++)
            {
                towerRendererList[i].material.color = GameManager.Instance.TowerPlaceSystem.GhostedTowerColor;
            }
        }

        private bool EndTowerBuild()
        {
            for (int i = 0; i < towerRendererList.Count; i++)
            {
                towerRendererList[i].material.color = Color.white - new Color(0.2f, 0.2f, 0.2f);
            }

            towerRangeTransform.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

            return true;
        }

        private void RotateTowerAtCreep()
        {
            var offset = rangeCollider.CreepInRangeList[0].transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            movingPartTransform.rotation = Quaternion.Lerp(movingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }

        private void RotateTowerToDefault()
        {
            if (movingPartTransform.rotation != Quaternion.Euler(0, 0, 0))
            {
                movingPartTransform.rotation = Quaternion.Lerp(movingPartTransform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 1f);
            }
        }

        private void Start()
        {
            towerStats = ScriptableObject.CreateInstance<TowerStats>();
            towerRendererList = new List<Renderer>();
                                
            towerStats.entityName = "SampleTowerName";
                                
            towerRendererList.AddRange(GetComponentsInChildren<Renderer>());
            
            towerRangeTransform = transform.GetChild(0);
            movingPartTransform = transform.GetChild(1);
            shootPointTransform = movingPartTransform.GetChild(0).GetChild(0);

            rangeCollider = towerRangeTransform.gameObject.GetComponent<RangeCollider>();

            var randomNumber = Random.Range(750, 900);

            towerRangeTransform.localScale = new Vector3(randomNumber, 0.0001f, randomNumber);

           

        }
   
        private void LateUpdate()
        {

            if (!IsTowerBuilded && GameManager.Instance.UISystem.IsBuildModeActive)
            {
                StartTowerBuild();
            }
            else
            {
                IsTowerBuilded = EndTowerBuild();
            }

            if (IsTowerBuilded)
            {
                if (rangeCollider.CreepInRangeList.Count > 0 && rangeCollider.CreepInRangeList[0] != null && rangeCollider.IsCreepInRange)
                {
                    RotateTowerAtCreep();
                    TowerCombatSystem.ShootAtCreep(5f);
                }
                else
                {
                    TowerCombatSystem.MoveBulletForward();
                    
                    RotateTowerToDefault();
                }
            }
        }

    }
}

//private void ShootAtCreep(float delay)
//{

//    if (timer < delay)
//    {
//        timer += 0.5f;

//        if (timer == 0.5f)
//        {
//            bulletList.Add(bulletPool.GetObject());
//            bulletTransformList.Add(bulletList[bulletList.Count - 1].transform);

//            bulletList[bulletList.Count - 1].transform.position = shootPointTransform.position;
//            bulletList[bulletList.Count - 1].transform.rotation = movingPartTransform.rotation;

//            bulletParticleSystemList.Add(bulletList[bulletList.Count - 1].GetComponentInChildren<ParticleSystem>());

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

//private IEnumerator RemoveBullets(float delay)
//{
//    yield return new WaitForSeconds(delay);

//    if (bulletList.Count > 0)
//    {
//        bulletList[0].SetActive(false);
//        bulletList.RemoveAt(0);
//        bulletParticleSystemList.RemoveAt(0);
//    }
//}
