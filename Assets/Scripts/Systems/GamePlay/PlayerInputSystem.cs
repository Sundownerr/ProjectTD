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
            GM.I.TowerUISystem.Selling += OnSelling;
            GM.I.TowerUISystem.Upgrading += OnUpgrading;
            GM.I.BuildUISystem.NeedToBuildTower += OnBuildNewTower;
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

                    if (isMouseOnTower)                                          
                        ActivateTowerUI(true);                          
                    
                    if (isMouseNotOnUI)
                        ActivateTowerUI(false);
                }
            }

            if (isHitUI)
            {
                results.Clear();
                isHitUI = false;
            }
        }

        private void OnSelling(object sender, EventArgs e) =>                 
            TowerSold?.Invoke(this, new TowerEventArgs(choosedTower, choosedTower.Stats));
            
        private bool CheckGradeListOk(out List<TowerData> grades)
        {
            var allTowerList = GM.I.TowerDataBase.AllTowers.
                Elements[(int)choosedTower.Stats.Element].
                Rarities[(int)choosedTower.Stats.Rarity].
                Towers;
               
            grades = allTowerList.Find(tower => 
                tower.CompareId(choosedTower.Stats.Id)).Grades;

            return grades.Count > 0 &&
                choosedTower.Stats.GradeCount < grades.Count - 1;
        }        

        private void OnUpgrading(object sender, EventArgs e) 
        {           
            if (CheckGradeListOk(out List<TowerData> grades))
            {              
                var upgradedTowerPrefab = Instantiate(
                    grades[choosedTower.Stats.GradeCount + 1].Prefab, 
                    choosedTower.Prefab.transform.position, 
                    Quaternion.identity, 
                    GM.I.TowerParent);
                var upgradedTower = new TowerSystem(upgradedTowerPrefab); 
                
                upgradedTower.StatsSystem.Upgrade(choosedTower, grades[choosedTower.Stats.GradeCount + 1]);                            
                upgradedTower.SetSystem();                                         
                
                TowerUpgraded?.Invoke(this, new TowerEventArgs(upgradedTower));      
                TowerSold?.Invoke(this, new TowerEventArgs(choosedTower, choosedTower.Stats));      
                choosedTower = upgradedTower;
            }
            GM.I.TowerUISystem.ActivateUpgradeButton(choosedTower.Stats.GradeCount < grades.Count - 1);
        }

        private void ActivateTowerUI(bool active)
        {             
            var isNotPlacingTower = 
                GM.PlayerState != State.PlacingTower && 
                GM.PlayerState != State.PreparePlacingTower;

            if (active)
            {  
                choosedTower = GM.I.Towers.Find(tower => tower.Prefab == hit.transform.gameObject);                      
                choosedTower.StatsSystem.StatsChanged += GM.I.TowerUISystem.UpdateValues;   
                GM.I.TowerUISystem.ActivateUpgradeButton(CheckGradeListOk(out _));      

                if (isNotPlacingTower)
                    GM.PlayerState = State.ChoosedTower;

                MouseOnTower?.Invoke(this, new EventArgs());           
            }
            else 
            {
                if (isNotPlacingTower)
                    GM.PlayerState = State.Idle;      

                if (choosedTower != null)
                    choosedTower.StatsSystem.StatsChanged -= GM.I.TowerUISystem.UpdateValues;        
            }  
        
            GM.I.TowerUISystem.gameObject.SetActive(active);           
        }       

        private void OnBuildNewTower(object sender, EventArgs e)
        {
            GM.PlayerState = State.PreparePlacingTower;

            for (int i = 0; i < GM.I.AvailableTowers.Count; i++)
                if (GM.I.AvailableTowers[i] == NewTowerData)
                {
                    GM.I.AvailableTowers.RemoveAt(i);
                    break;
                }           
            StartedTowerBuild?.Invoke(this, new EventArgs());
       }    
    }
}
