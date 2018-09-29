using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;
using Game.Tower.CombatSystem;
#pragma warning disable CS1591 
namespace Game.Tower
{

    public class Tower : ExtendedMonoBehaviour
    {
        public GameObject Bullet, TowerPlaceEffect;
        public bool IsTowerBuilded;
        public TowerStats towerStats;
        public Transform towerRangeTransform, movingPartTransform, shootPointTransform;
        public TowerRange rangeCollider;
        public TowerCombatSystem TowerCombatSystem;

        private List<Renderer> towerRendererList;
        private bool isRangeShowed;
       

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

                towerRangeTransform.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

                gameObject.layer = 14;

                var towerPlaceEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90,0,0));
                Destroy(towerPlaceEffect, 2f);

                return true;
            }

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

        private bool ShowTowerRange(bool isShowed)
        {
            if (!isShowed)
            {
                towerRangeTransform.GetComponent<Renderer>().material.color = new Color(0, 0.5f, 0, 0.2f);
                return true;
            }
            else
            {
                towerRangeTransform.GetComponent<Renderer>().material.color = new Color(0, 0f, 0, 0f);
                return false;
            }
        }

        private void Start()
        {
            
            towerStats = ScriptableObject.CreateInstance<TowerStats>();
            towerRendererList = new List<Renderer>();
                                
            towerStats.entityName = "SampleTowerName" +Random.Range(100, 1000);
            towerStats.damage = Random.Range(1000,10000);
            towerStats.range = Random.Range(510, 900);
            towerStats.critChance = Mathf.Floor(Random.Range(0, 1f) * 100);
            towerStats.mana = Random.Range(0, 100000);
            towerStats.spellDamage = Mathf.Floor(Random.Range(0, 10f) * 100);
            towerStats.triggerChance = Mathf.Floor(Random.Range(0, 1f) * 100);

            towerRendererList.AddRange(GetComponentsInChildren<Renderer>());
            
            towerRangeTransform = transform.GetChild(0);
            movingPartTransform = transform.GetChild(1);
            shootPointTransform = movingPartTransform.GetChild(0).GetChild(0);

            rangeCollider = towerRangeTransform.gameObject.GetComponent<TowerRange>();

            var randomNumber = Random.Range(510, 900);

            towerRangeTransform.localScale = new Vector3(towerStats.range, 0.001f, towerStats.range);
            
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
                    TowerCombatSystem.ShootAtCreep(0.3f);
                }
                else
                {
                    TowerCombatSystem.MoveBulletOutOfRange();
                    
                    RotateTowerToDefault();
                }

                if(GameManager.Instance.UISystem.IsBuildModeActive && !isRangeShowed)
                {
                    ShowTowerRange(false);
                }
                else
                {
                    ShowTowerRange(true);
                }
            }
        }
    }
}