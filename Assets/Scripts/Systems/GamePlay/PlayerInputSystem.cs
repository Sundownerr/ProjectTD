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
        public event EventHandler<ElementType> StartedTowerBuild = delegate {};
        
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

            GM.Instance.PlayerInputSystem = this;

            state = new StateMachine();
            state.ChangeState(new GetInputState(this));
        }

        private void Start()
        {
            GM.Instance.TowerUISystem.Selling += SellTower;
            GM.Instance.TowerUISystem.Upgrading += UpgradeTower;
            GM.Instance.BuildUISystem.NeedToBuildTower += BuildNewTower;
        }

        private void Update() => state.Update();

        private void SellTower(object sender, EventArgs e)
        {
            GM.Instance.ResourceSystem.AddTowerLimit(-ChoosedTower.Stats.TowerLimit);
            GM.Instance.ResourceSystem.AddGold(ChoosedTower.Stats.GoldCost);

            ChoosedTower.OcuppiedCell.GetComponent<Cells.Cell>().IsBusy = false;
            GM.Instance.PlacedTowerList.Remove(ChoosedTower.gameObject);
            Destroy(ChoosedTower.gameObject);
        }                 

        private void UpgradeTower(object sender, EventArgs e) 
        {
            var isGradeCountOk =
                ChoosedTower.Stats.GradeList.Count > 0 &&
                ChoosedTower.Stats.GradeCount < ChoosedTower.Stats.GradeList.Count;

            if (isGradeCountOk)
            {
                var upgradedTowerPrefab = Instantiate(ChoosedTower.Stats.GradeList[0].Prefab, ChoosedTower.transform.position, Quaternion.identity, GM.Instance.TowerParent);
                var upgradedTowerSystem = upgradedTowerPrefab.GetComponent<TowerSystem>();

                upgradedTowerSystem.StatsSystem.Upgrade(ChoosedTower.Stats, ChoosedTower.Stats.GradeList[0]);
                upgradedTowerSystem.OcuppiedCell = ChoosedTower.OcuppiedCell;
                upgradedTowerSystem.SetSystem();

                GM.Instance.PlayerInputSystem.ChoosedTower = upgradedTowerSystem;
                ChoosedTower.StatsSystem.OnStatsChanged();
                
                Destroy(gameObject);
            }
        }

        private void ActivateTowerUI(bool active)
        {             
            var isNotPlacingTower = 
                GM.PlayerState != State.PlacingTower && 
                GM.PlayerState != State.PreparePlacingTower;

            if(active)
            {                            
                if (isNotPlacingTower)
                    GM.PlayerState = State.ChoosedTower;

                ChoosedTower = hit.transform.gameObject.GetComponent<TowerSystem>();  
                MouseOnTower?.Invoke(this, new EventArgs());
            }
            else 
                if (isNotPlacingTower)
                    GM.PlayerState = State.Idle;           

            GM.Instance.TowerUISystem.gameObject.SetActive(active);
        }       

        private void BuildNewTower(object sender, EventArgs e)
        {
            GM.PlayerState = State.PreparePlacingTower;

            for (int i = 0; i < GM.Instance.AvailableTowerList.Count; i++)
                if (GM.Instance.AvailableTowerList[i] == NewTowerData)
                {
                    GM.Instance.AvailableTowerList.RemoveAt(i);
                    break;
                }
            
            StartedTowerBuild?.Invoke(this, GM.Instance.PlayerInputSystem.NewTowerData.Element);
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
