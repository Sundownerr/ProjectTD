using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Tower.Data;
using System;
using Game.Tower;
using Game.Tower.Data.Stats;

namespace Game.Systems
{
    public class PlayerInputSystem : ExtendedMonoBehaviour
    {
        public TowerSystem ChoosedTower { get => choosedTower; set => choosedTower = value; }
        public TowerData NewTowerData { get => newTowerData; set => newTowerData = value; }

        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;
        public event EventHandler MouseOnTower = delegate {};
        public event EventHandler StartedTowerBuild = delegate {};
        public event EventHandler<TowerEventArgs> TowerSold = delegate{};
        public event EventHandler<TowerEventArgs> TowerUpgraded = delegate{};
        
        private TowerData newTowerData;
        private TowerSystem choosedTower;
        private PointerEventData pointerEventData;
        private List<RaycastResult> results;
        private RaycastHit hit;
        private Ray WorldRay;
        private bool isHitUI;
        private int terrainLayer, creepLayer, towerLayer, layerMask;
       
        protected override void Awake()
        {
            base.Awake();
           
            results = new List<RaycastResult>();

            GM.I.PlayerInputSystem = this;

            terrainLayer    = 1 << 9;
            creepLayer      = 1 << 12;
            towerLayer      = 1 << 14;           
            layerMask = terrainLayer | creepLayer | towerLayer;
        }

        private void Start()
        {
            GM.I.TowerUISystem.Selling += SellTower;
            GM.I.TowerUISystem.Upgrading += UpgradeTower;
            GM.I.BuildUISystem.NeedToBuildTower += BuildNewTower;
        }

        private void Update() 
        {      
            if (Input.GetMouseButtonDown(0))
                {                   
                    WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    pointerEventData = new PointerEventData(EventSystem);
                    pointerEventData.position = Input.mousePosition;
                    GraphicRaycaster.Raycast(pointerEventData, results);
                    isHitUI = results.Count > 0;

                    if (Physics.Raycast(WorldRay, out hit, 10000, layerMask))
                    {
                        var isMouseOnTower =
                            !isHitUI &&
                            hit.transform.gameObject.layer == 14;

                        var isMouseNotOnUI =
                            !isHitUI &&
                            hit.transform.gameObject.layer == 9;

                        if(isMouseOnTower)                                          
                            ActivateTowerUI(true);                          
                        
                        if(isMouseNotOnUI)
                            ActivateTowerUI(false);
                    }
                }

                if (isHitUI)
                {
                    results.Clear();
                    isHitUI = false;
                }
        }

        private void SellTower(object sender, EventArgs e)
        {
            TowerSold?.Invoke(this, new TowerEventArgs(ChoosedTower.Stats, ChoosedTower.Stats.TowerLimit, ChoosedTower.Stats.GoldCost));
            ChoosedTower.OcuppiedCell.GetComponent<Cells.Cell>().IsBusy = false;          
            GM.I.PlacedTowerList.Remove(ChoosedTower.gameObject);
            ChoosedTower.Stats.Destroy();
            Destroy(ChoosedTower.gameObject);
        }         

        private bool CheckGradeListOk(out List<TowerData> gradeList)
        {
            var towerDBList = GM.I.TowerDataBase.AllTowerList.ElementsList[(int)choosedTower.Stats.Element].RarityList[(int)choosedTower.Stats.Rarity].TowerList;
            gradeList = towerDBList.Find(tower => tower.CompareId(choosedTower.Stats.Id)).GradeList;

            return gradeList.Count > 0 &&
                choosedTower.Stats.GradeCount < gradeList.Count - 1;
        }        

        private void UpgradeTower(object sender, EventArgs e) 
        {           
            if (CheckGradeListOk(out List<TowerData> gradeList))
            {              
                var upgradedTowerPrefab = Instantiate(
                    gradeList[choosedTower.Stats.GradeCount + 1].Prefab, 
                    choosedTower.transform.position, 
                    Quaternion.identity, 
                    GM.I.TowerParent);
                var upgradedTower = upgradedTowerPrefab.GetComponent<TowerSystem>(); 
                
                upgradedTower.StatsSystem.Upgrade(choosedTower, gradeList[choosedTower.Stats.GradeCount + 1]);                            
                upgradedTower.SetSystem();   
                            
                TowerUpgraded?.Invoke(this, new TowerEventArgs(upgradedTower));
                choosedTower = upgradedTower;
            }
            GM.I.TowerUISystem.UpgradeButton.gameObject.SetActive(choosedTower.Stats.GradeCount < gradeList.Count - 1);
        }

        private void ActivateTowerUI(bool active)
        {             
            var isNotPlacingTower = 
                GM.PlayerState != State.PlacingTower && 
                GM.PlayerState != State.PreparePlacingTower;

            if (active)
            {  
                choosedTower = hit.transform.GetComponent<TowerSystem>();                            
                if (isNotPlacingTower)
                    GM.PlayerState = State.ChoosedTower;

                GM.I.TowerUISystem.UpgradeButton.gameObject.SetActive(CheckGradeListOk(out _));           
                MouseOnTower?.Invoke(this, new EventArgs());
            }
            else 
                if (isNotPlacingTower)
                    GM.PlayerState = State.Idle;           
        
            GM.I.TowerUISystem.gameObject.SetActive(active);           
        }       

        private void BuildNewTower(object sender, EventArgs e)
        {
            GM.PlayerState = State.PreparePlacingTower;

            for (int i = 0; i < GM.I.AvailableTowerList.Count; i++)
                if (GM.I.AvailableTowerList[i] == NewTowerData)
                {
                    GM.I.AvailableTowerList.RemoveAt(i);
                    break;
                }           
            StartedTowerBuild?.Invoke(this, new EventArgs());
       }    
    }
}
