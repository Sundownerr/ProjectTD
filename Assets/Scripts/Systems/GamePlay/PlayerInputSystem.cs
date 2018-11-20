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
        
        private TowerData newTowerData;
        private TowerSystem choosedTower;
        private PointerEventData pointerEventData;
        private List<RaycastResult> results;
        private StateMachine state;
        private RaycastHit hit;
        private Ray WorldRay;
        private bool isHitUI;
       
        protected override void Awake()
        {
            base.Awake();
           
            results = new List<RaycastResult>();

            GM.I.PlayerInputSystem = this;

            state = new StateMachine();
            state.ChangeState(new GetInputState(this));
        }

        private void Start()
        {
            GM.I.TowerUISystem.Selling += SellTower;
            GM.I.TowerUISystem.Upgrading += UpgradeTower;
            GM.I.BuildUISystem.NeedToBuildTower += BuildNewTower;
        }

        private void Update() => state.Update();
  

        private void SellTower(object sender, EventArgs e)
        {
            GM.I.ResourceSystem.AddTowerLimit(-ChoosedTower.Stats.TowerLimit);
            GM.I.ResourceSystem.AddGold(ChoosedTower.Stats.GoldCost);

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
                var upgradedTowerPrefab = Instantiate(gradeList[choosedTower.Stats.GradeCount + 1].Prefab, choosedTower.transform.position, Quaternion.identity, GM.I.TowerParent);
                var upgradedTowerSystem = upgradedTowerPrefab.GetComponent<TowerSystem>(); 
                
                upgradedTowerSystem.StatsSystem.Upgrade(choosedTower.Stats, gradeList[choosedTower.Stats.GradeCount + 1]);
                upgradedTowerSystem.OcuppiedCell = choosedTower.OcuppiedCell;
                upgradedTowerSystem.SetSystem();   
                upgradedTowerSystem.IsTowerPlaced = true;
                      
                choosedTower.Stats.Destroy();
                Destroy(choosedTower.gameObject);
                
                choosedTower = upgradedTowerSystem;
                choosedTower.StatsSystem.OnStatsChanged();      
                GM.I.TowerControlSystem.AddTower(choosedTower);
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
       
        protected class GetInputState : IState
        {
            private readonly PlayerInputSystem o;

            public GetInputState(PlayerInputSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute()
            {
                var terrainLayer    = 1 << 9;
                var creepLayer      = 1 << 12;
                var towerLayer      = 1 << 14;
                var layerMask       = terrainLayer | creepLayer | towerLayer;

                o.WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                o.pointerEventData = new PointerEventData(o.EventSystem);
                o.pointerEventData.position = Input.mousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    o.GraphicRaycaster.Raycast(o.pointerEventData, o.results);
                    o.isHitUI = o.results.Count > 0;

                    if (Physics.Raycast(o.WorldRay, out o.hit, 10000, layerMask))
                    {
                        var isMouseOnTower =
                            !o.isHitUI &&
                            o.hit.transform.gameObject.layer == 14;

                        var isMouseNotOnUI =
                            !o.isHitUI &&
                            o.hit.transform.gameObject.layer == 9;

                        if(isMouseOnTower)                                          
                            o.ActivateTowerUI(true);                          
                        
                        if(isMouseNotOnUI)
                            o.ActivateTowerUI(false);
                    }
                }

                if (o.isHitUI)
                {
                    o.results.Clear();
                    o.isHitUI = false;
                }
            }
            
            public void Exit() { }
        }
    }
}
