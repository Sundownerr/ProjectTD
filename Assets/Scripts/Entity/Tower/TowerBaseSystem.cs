using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;


namespace Game.Tower
{

    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
        public GameObject Bullet, TowerPlaceEffect;
        public bool IsTowerBuilded;
        public TowerStats TowerStats;
        public Transform towerRangeTransform, movingPartTransform, shootPointTransform;
        public TowerRangeSystem TowerRange;
        public TowerCombatSystem TowerCombatSystem;

        private List<Renderer> towerRendererList;       

        private void StartTowerBuild()
        {
            for (int i = 0; i < towerRendererList.Count; i++)
            {
                towerRendererList[i].material.color = GameManager.Instance.TowerPlaceSystem.GhostedTowerColor;
            }
        }

        private bool EndTowerBuild()
        {
            if (!IsTowerBuilded)
            {
                for (int i = 0; i < towerRendererList.Count; i++)
                {
                    towerRendererList[i].material.color = Color.white - new Color(0.2f, 0.2f, 0.2f);
                }

                TowerRange.Show(false);

                gameObject.layer = 14;

                var towerPlaceEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90,0,0));
                Destroy(towerPlaceEffect, 2f);

                return true;
            }

            return true;
        }

        private void RotateTowerAtCreep()
        {
            var offset = TowerRange.CreepInRangeList[0].transform.position - transform.position;
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
            
            TowerStats = ScriptableObject.CreateInstance<TowerStats>();
            towerRendererList = new List<Renderer>();
                                
            TowerStats.entityName = "SampleTowerName" +Random.Range(100, 1000);
            TowerStats.damage = Random.Range(1000,10000);
            TowerStats.range = Random.Range(510, 900);
            TowerStats.critChance = Mathf.Floor(Random.Range(0, 1f) * 100);
            TowerStats.mana = Random.Range(0, 100000);
            TowerStats.spellDamage = Mathf.Floor(Random.Range(0, 10f) * 100);
            TowerStats.triggerChance = Mathf.Floor(Random.Range(0, 1f) * 100);

            towerRendererList.AddRange(GetComponentsInChildren<Renderer>());
            
            towerRangeTransform = transform.GetChild(0);
            movingPartTransform = transform.GetChild(1);
            shootPointTransform = movingPartTransform.GetChild(0).GetChild(0);

            TowerRange = towerRangeTransform.gameObject.GetComponent<TowerRangeSystem>();

            towerRangeTransform.localScale = new Vector3(TowerStats.range, 0.001f, TowerStats.range);
            
        }
       
        private void Update()
        {
            if (!IsTowerBuilded && GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
            {
                StartTowerBuild();
            }
            else
            {
                IsTowerBuilded = EndTowerBuild();              
            }

            if (IsTowerBuilded)
            {
                if (TowerRange.CreepInRangeList.Count > 0 && TowerRange.CreepInRangeList[0] != null && TowerRange.IsCreepInRange)
                {
                    RotateTowerAtCreep();
                    TowerCombatSystem.ShootAtCreep(0.2f);
                }
                else
                {
                    TowerCombatSystem.MoveBulletOutOfRange();
                    
                    RotateTowerToDefault();
                }


                if(GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    TowerRange.Show(true);
                }
                else
                {
                    if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_CHOOSEDTOWER)
                    {
                        TowerRange.Show(false);
                    }                   
                }
            }
        }
    }
}