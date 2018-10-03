using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;


namespace Game.Tower
{

    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
        public Transform RangeTransform, MovingPartTransform, ShootPointTransform;
        public GameObject Bullet, TowerPlaceEffect, OcuppiedCell, Range;
        public TowerCombatSystem CombatSystem;
        public TowerRangeSystem RangeSystem;
        public TowerStats Stats, BaseStats;
        public bool IsPlaced;
    
        private List<Renderer> rendererList;

        private void Start()
        {
            Stats = Instantiate(BaseStats);

            CombatSystem = GetComponent<TowerCombatSystem>();
            Range = Instantiate(GameManager.Instance.RangePrefab, transform);
            RangeSystem = Range.GetComponent<TowerRangeSystem>();
            Range.transform.localScale = new Vector3(Stats.Range, 0.001f, Stats.Range);

            rendererList = new List<Renderer>();
            rendererList.AddRange(GetComponentsInChildren<Renderer>());
                   
            MovingPartTransform = transform.GetChild(0);
            ShootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);            
        }

        private void Update()
        {


            if (!IsPlaced && GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
            {
                StartPlacing();
            }
            else
            {
                EndPlacing();
            }

            if (IsPlaced)
            {
                if (RangeSystem.CreepInRangeList.Count > 0 && RangeSystem.CreepInRangeList[0] != null)
                {
                    RotateAtCreep();

                    CombatSystem.Shoot(Stats.AttackSpeed);
                }
                else
                {
                    if (!CombatSystem.CheckAllBulletInactive())
                    {
                        CombatSystem.MoveBulletOutOfRange();
                    }
                }

                if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    RangeSystem.Show(true);
                }
                else if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_CHOOSEDTOWER)
                {
                    RangeSystem.Show(false);
                }
            }
        }

        private void SetTowerColor(Color color)
        {
            for (int i = 0; i < rendererList.Count; i++)
            {
                rendererList[i].material.color = color;
            }
        }

        private void StartPlacing()
        {
            SetTowerColor(GameManager.Instance.TowerPlaceSystem.GhostedTowerColor);

            transform.position = GameManager.Instance.TowerPlaceSystem.GhostedTowerPos;
        }

        private void EndPlacing()
        {
            if (!IsPlaced)
            {
                OcuppiedCell = GameManager.Instance.TowerPlaceSystem.NewBusyCell;

                if (transform.position != OcuppiedCell.transform.position)
                {
                    transform.position = OcuppiedCell.transform.position;
                }              

                SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));

                var placeEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90, 0, 0));
                Destroy(placeEffect, 1f);

                gameObject.layer = 14;
                RangeSystem.Show(false);
                            
                IsPlaced = true;
            }
        }

        private void RotateAtCreep()
        {
            var offset = RangeSystem.CreepInRangeList[0].transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            MovingPartTransform.rotation = Quaternion.Lerp(MovingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }            

        public void Sell()
        {
            OcuppiedCell.GetComponent<TowerCells.Cell>().IsBusy = false;
            GameManager.Instance.TowerList.Remove(gameObject);
            Destroy(gameObject);            
        }
    }
}