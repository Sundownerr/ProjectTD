using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;


namespace Game.Tower
{

    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
        public Transform towerRangeTransform, movingPartTransform, shootPointTransform;
        public GameObject Bullet, TowerPlaceEffect, OcuppiedCell, Range;
        public TowerCombatSystem TowerCombatSystem;
        public TowerRangeSystem TowerRange;
        public TowerStats TowerStats;
        public bool IsTowerPlaced;
    
        private List<Renderer> towerRendererList;
        private bool isRangeShowed;

        private void Start()
        {
            Range = Instantiate(GameManager.Instance.RangePrefab, transform);

            TowerStats = ScriptableObject.CreateInstance<TowerStats>();
            towerRendererList = new List<Renderer>();

            TowerStats.entityName = "SampleTowerName" + Random.Range(100, 1000);
            TowerStats.Damage = Random.Range(10, 20);
            TowerStats.Range = Random.Range(510, 900);
            TowerStats.CritChance = Mathf.Floor(Random.Range(0, 1f) * 100);
            TowerStats.Mana = Random.Range(0, 100000);
            TowerStats.SpellDamage = Mathf.Floor(Random.Range(0, 10f) * 100);
            TowerStats.TriggerChance = Mathf.Floor(Random.Range(0, 1f) * 100);
            TowerStats.AttackSpeed = Random.Range(0.1f, 1f);

            towerRendererList.AddRange(GetComponentsInChildren<Renderer>());

            towerRangeTransform = Range.transform;
            movingPartTransform = transform.GetChild(1);
            shootPointTransform = movingPartTransform.GetChild(0).GetChild(0);

            TowerRange = Range.GetComponent<TowerRangeSystem>();

            towerRangeTransform.localScale = new Vector3(TowerStats.Range, 0.001f, TowerStats.Range);
        }

        private void Update()
        {
            if (!IsTowerPlaced && GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
            {
                StartTowerPlace();
            }
            else
            {
                EndTowerPlace();
            }

            if (IsTowerPlaced)
            {
                if (TowerRange.CreepInRangeList.Count > 0 && TowerRange.CreepInRangeList[0] != null)
                {
                    RotateTowerAtCreep();

                    TowerCombatSystem.ShootAtCreep(TowerStats.AttackSpeed);
                }
                else 
                {                  
                    TowerCombatSystem.MoveBulletOutOfRange();
                }

                if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    TowerRange.Show(true);
                }
                else if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_CHOOSEDTOWER)
                {
                    TowerRange.Show(false);
                }
            }
        }

        private void SetTowerColor(Color color)
        {
            for (int i = 0; i < towerRendererList.Count; i++)
            {
                towerRendererList[i].material.color = color;
            }
        }

        private void StartTowerPlace()
        {
            SetTowerColor(GameManager.Instance.TowerPlaceSystem.GhostedTowerColor);

            transform.position = GameManager.Instance.TowerPlaceSystem.GhostedTowerPos;
        }

        private void EndTowerPlace()
        {
            if (!IsTowerPlaced)
            {
                var towerPlaceEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90, 0, 0));
                Destroy(towerPlaceEffect, 1f);

                OcuppiedCell = GameManager.Instance.TowerPlaceSystem.NewBusyCell;

                SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));               

                if (transform.position != OcuppiedCell.transform.position)
                {
                    transform.position = OcuppiedCell.transform.position;
                }

                gameObject.layer = 14;
                TowerRange.Show(false);
                            
                IsTowerPlaced = true;
            }
        }

        private void RotateTowerAtCreep()
        {
            var offset = TowerRange.CreepInRangeList[0].transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            movingPartTransform.rotation = Quaternion.Lerp(movingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }            
    }
}