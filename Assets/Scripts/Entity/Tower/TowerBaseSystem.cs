using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;


namespace Game.Tower
{

    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
        public GameObject Bullet, TowerPlaceEffect, OcuppiedCell;
        public bool IsTowerPlaced;
        public TowerStats TowerStats;
        public Transform towerRangeTransform, movingPartTransform, shootPointTransform;
        public TowerRangeSystem TowerRange;
        public TowerCombatSystem TowerCombatSystem;
        private List<Renderer> towerRendererList;           

        private void StartTowerPlace()
        {

            
            for (int i = 0; i < towerRendererList.Count; i++)
            {
                towerRendererList[i].material.color = GameManager.Instance.TowerPlaceSystem.GhostedTowerColor;
                //towerRendererList[i].enabled = GameManager.Instance.TowerPlaceSystem.GhostedTowerVisible;
            }

            transform.position = GameManager.Instance.TowerPlaceSystem.GhostedTowerPos;
        }

        private bool EndTowerPlace()
        {
            if (!IsTowerPlaced)
            {
                for (int i = 0; i < towerRendererList.Count; i++)
                {
                    towerRendererList[i].material.color = Color.white - new Color(0.2f, 0.2f, 0.2f);
                }

                TowerRange.Show(false);

                gameObject.layer = 14;

                var towerPlaceEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90,0,0));
                Destroy(towerPlaceEffect, 2f);

                OcuppiedCell = GameManager.Instance.TowerPlaceSystem.NewBusyCell;

                if (transform.position != OcuppiedCell.transform.position)
                {
                    transform.position = OcuppiedCell.transform.position;
                }

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
            TowerStats.Damage = Random.Range(10,20);
            TowerStats.Range = Random.Range(510, 900);
            TowerStats.CritChance = Mathf.Floor(Random.Range(0, 1f) * 100);
            TowerStats.Mana = Random.Range(0, 100000);
            TowerStats.SpellDamage = Mathf.Floor(Random.Range(0, 10f) * 100);
            TowerStats.TriggerChance = Mathf.Floor(Random.Range(0, 1f) * 100);

            towerRendererList.AddRange(GetComponentsInChildren<Renderer>());
            
            towerRangeTransform = transform.GetChild(0);
            movingPartTransform = transform.GetChild(1);
            shootPointTransform = movingPartTransform.GetChild(0).GetChild(0);

            TowerRange = towerRangeTransform.gameObject.GetComponent<TowerRangeSystem>();

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
                IsTowerPlaced = EndTowerPlace();    
                
            }

            if (IsTowerPlaced)
            {               
                if (TowerRange.CreepInRangeList.Count > 0 && TowerRange.CreepInRangeList[0] != null && TowerRange.IsCreepInRange)
                {
                    RotateTowerAtCreep();

                    TowerCombatSystem.ShootAtCreep(0.7f);
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