using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.System
{
    public class PlayerInputSystem : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public Data.Entity.Tower.TowerData NewTowerData;

        [HideInInspector]
        public GameObject ChoosedTower;

        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;
        
        private PointerEventData pointerEventData;
        private List<RaycastResult> results;
        private StateMachine state;
        private RaycastHit hit;
        private Ray WorldRay;
        private bool isHitUI;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
                CachedTransform = transform;
           
            results = new List<RaycastResult>();

            GM.Instance.PlayerInputSystem = this;

            state = new StateMachine();
            state.ChangeState(new GetInputState(this));
        }

        private void Update()
        {
            state.Update();
        }

        protected class GetInputState : IState
        {
            private readonly PlayerInputSystem owner;

            public GetInputState(PlayerInputSystem owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                var terrainLayer = 1 << 9;
                var creepLayer = 1 << 12;
                var towerLayer = 1 << 14;
                var layerMask = terrainLayer | creepLayer | towerLayer;

                owner.WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                var isRayHit = Physics.Raycast(owner.WorldRay, out owner.hit, 10000, layerMask);

                owner.pointerEventData = new PointerEventData(owner.EventSystem);
                owner.pointerEventData.position = Input.mousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    owner.GraphicRaycaster.Raycast(owner.pointerEventData, owner.results);

                    if (owner.results.Count > 0)
                        owner.isHitUI = true;

                    if (isRayHit)
                    {
                        var isMouseOnTower =
                            !owner.isHitUI &&
                            owner.hit.transform.gameObject.layer == 14;

                        var isMouseNotOnUI =
                            !owner.isHitUI &&
                            owner.hit.transform.gameObject.layer == 9;

                        if (isMouseOnTower)
                            owner.state.ChangeState(new MouseOnTowerState(owner));

                        if (isMouseNotOnUI)
                            owner.state.ChangeState(new MouseNotOnUIState(owner));
                    }
                }

                if (owner.results.Count > 0)
                {
                    owner.results.Clear();
                    owner.isHitUI = false;
                }

                if(GM.Instance.TowerUISystem.IsSellig)
                    owner.state.ChangeState(new SellTowerState(owner));

                if (GM.Instance.TowerUISystem.IsUpgrading)
                    owner.state.ChangeState(new UpgradeTowerState(owner));

                if (GM.Instance.BuildUISystem.IsChoosedNewTower)
                    owner.state.ChangeState(new CreateNewTowerState(owner));
            }
            
            public void Exit() { }
        }

        protected class MouseOnTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public MouseOnTowerState(PlayerInputSystem owner) { this.owner = owner; }

            public void Enter()
            {
                var towerUI = GM.Instance.TowerUISystem;

                owner.ChoosedTower = owner.hit.transform.gameObject;

                if (!towerUI.gameObject.activeSelf)
                    towerUI.gameObject.SetActive(true);

                owner.StartCoroutine(towerUI.RefreshUI());

                if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
                    GM.PLAYERSTATE = GM.CHOOSED_TOWER;

                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class MouseNotOnUIState : IState
        {
            private readonly PlayerInputSystem owner;

            public MouseNotOnUIState(PlayerInputSystem owner) { this.owner = owner; }

            public void Enter()
            {
                var towerUI = GM.Instance.TowerUISystem;

                if (towerUI.gameObject.activeSelf)
                    towerUI.gameObject.SetActive(false);
                
                if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
                    GM.PLAYERSTATE = GM.IDLE;

                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class SellTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public SellTowerState(PlayerInputSystem owner) { this.owner = owner; }

            public void Enter()
            {
                owner.ChoosedTower.GetComponent<Tower.TowerBaseSystem>().Sell();
                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.TowerUISystem.IsSellig = false;
                GM.Instance.TowerUISystem.gameObject.SetActive(false);
            }
        }

        protected class UpgradeTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public UpgradeTowerState(PlayerInputSystem owner) { this.owner = owner; }

            public void Enter()
            {
                owner.ChoosedTower.GetComponent<Tower.TowerBaseSystem>().Upgrade();
                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.TowerUISystem.IsUpgrading = false;                
            }
        }

        protected class CreateNewTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public CreateNewTowerState(PlayerInputSystem owner) { this.owner = owner; }

            public void Enter()
            {               
                GM.PLAYERSTATE = GM.PREPARE_PLACING_TOWER;

                for (int i = 0; i < GM.Instance.AvailableTowerList.Count; i++)
                {
                    if (GM.Instance.AvailableTowerList[i] == owner.NewTowerData)
                    {
                        GM.Instance.AvailableTowerList.RemoveAt(i);
                        break;
                    }
                }

                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.ElementId);

                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.BuildUISystem.IsChoosedNewTower = false;
            }
        }
    }
}
