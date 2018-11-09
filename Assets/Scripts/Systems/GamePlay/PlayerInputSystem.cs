using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Tower.Data;
using System;
using Game.Tower;

namespace Game.Systems
{
    public class PlayerInputSystem : ExtendedMonoBehaviour
    {
        public TowerSystem ChoosedTower { get => choosedTower; set => choosedTower = value; }
        public TowerData NewTowerData { get => newTowerData; set => newTowerData = value; }

        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;
        public event EventHandler MouseOnTower = delegate {};
        
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
        }

        private void Update() => state.Update();

        private void SellTower(object sender, EventArgs e) => ChoosedTower.Sell();                 

        private void UpgradeTower(object sender, EventArgs e) => ChoosedTower.Upgrade();    

        private void ActivateTowerUI()
        {             
            ChoosedTower = hit.transform.gameObject.GetComponent<TowerSystem>();   
            GM.Instance.TowerUISystem.gameObject.SetActive(true);

            if (GM.PlayerState != State.PlacingTower && GM.PlayerState != State.PreparePlacingTower)
                GM.PlayerState = State.ChoosedTower;

            MouseOnTower?.Invoke(this, new EventArgs());
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
                var isRayHit = Physics.Raycast(o.WorldRay, out o.hit, 10000, layerMask);

                o.pointerEventData = new PointerEventData(o.EventSystem);
                o.pointerEventData.position = Input.mousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    o.GraphicRaycaster.Raycast(o.pointerEventData, o.results);

                    if (o.results.Count > 0)
                        o.isHitUI = true;

                    if (isRayHit)
                    {
                        var isMouseOnTower =
                            !o.isHitUI &&
                            o.hit.transform.gameObject.layer == 14;

                        var isMouseNotOnUI =
                            !o.isHitUI &&
                            o.hit.transform.gameObject.layer == 9;

                        if (isMouseOnTower)
                            o.ActivateTowerUI();

                        if (isMouseNotOnUI)
                            o.state.ChangeState(new MouseNotOnUIState(o));
                    }
                }

                if (o.results.Count > 0)
                {
                    o.results.Clear();
                    o.isHitUI = false;
                }

                if (GM.Instance.BuildUISystem.IsChoosedNewTower)
                    o.state.ChangeState(new CreateNewTowerState(o));
            }
            
            public void Exit() { }
        }

        protected class MouseNotOnUIState : IState
        {
            private readonly PlayerInputSystem o;

            public MouseNotOnUIState(PlayerInputSystem o) => this.o = o; 
            
            public void Enter()
            {
                if (GM.Instance.TowerUISystem.gameObject.activeSelf)
                    GM.Instance.TowerUISystem.gameObject.SetActive(false);
                
                if (GM.PlayerState != State.PlacingTower && GM.PlayerState != State.PreparePlacingTower)
                    GM.PlayerState = State.Idle;

                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CreateNewTowerState : IState
        {
            private readonly PlayerInputSystem o;

            public CreateNewTowerState(PlayerInputSystem o) => this.o = o; 

            public void Enter()
            {               
                GM.PlayerState = State.PreparePlacingTower;

                for (int i = 0; i < GM.Instance.AvailableTowerList.Count; i++)
                    if (GM.Instance.AvailableTowerList[i] == o.NewTowerData)
                    {
                        GM.Instance.AvailableTowerList.RemoveAt(i);
                        break;
                    }
                
                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.Element);

                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.BuildUISystem.IsChoosedNewTower = false;
            }
        }
    }
}
