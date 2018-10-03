﻿using System.Collections;
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
        public TowerCombatSystem TowerCombatSystem;
        public TowerRangeSystem RangeSystem;
        public TowerStats TowerStats;
        public bool IsPlaced;
    
        private List<Renderer> towerRendererList;

        private void Start()
        {
            Range = Instantiate(GameManager.Instance.RangePrefab, transform);
            RangeSystem = Range.GetComponent<TowerRangeSystem>();
            Range.transform.localScale = new Vector3(TowerStats.Range, 0.001f, TowerStats.Range);

            towerRendererList = new List<Renderer>();
            towerRendererList.AddRange(GetComponentsInChildren<Renderer>());
                   
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

                    TowerCombatSystem.Shoot(TowerStats.AttackSpeed);
                }
                else
                {
                    if (!TowerCombatSystem.IsAllBulletsInactive)
                    {
                        TowerCombatSystem.MoveBulletOutOfRange();
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
            for (int i = 0; i < towerRendererList.Count; i++)
            {
                towerRendererList[i].material.color = color;
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
                var placeEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90, 0, 0));
                Destroy(placeEffect, 1f);

                OcuppiedCell = GameManager.Instance.TowerPlaceSystem.NewBusyCell;

                SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));               

                if (transform.position != OcuppiedCell.transform.position)
                {
                    transform.position = OcuppiedCell.transform.position;
                }

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