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
            private readonly PlayerInputSystem o;

            public GetInputState(PlayerInputSystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                var terrainLayer = 1 << 9;
                var creepLayer = 1 << 12;
                var towerLayer = 1 << 14;
                var layerMask = terrainLayer | creepLayer | towerLayer;

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
                            o.state.ChangeState(new MouseOnTowerState(o));

                        if (isMouseNotOnUI)
                            o.state.ChangeState(new MouseNotOnUIState(o));
                    }
                }

                if (o.results.Count > 0)
                {
                    o.results.Clear();
                    o.isHitUI = false;
                }

                if(GM.Instance.TowerUISystem.IsSellig)
                    o.state.ChangeState(new SellTowerState(o));

                if (GM.Instance.TowerUISystem.IsUpgrading)
                    o.state.ChangeState(new UpgradeTowerState(o));

                if (GM.Instance.BuildUISystem.IsChoosedNewTower)
                    o.state.ChangeState(new CreateNewTowerState(o));
            }
            
            public void Exit() { }
        }

        protected class MouseOnTowerState : IState
        {
            private readonly PlayerInputSystem o;

            public MouseOnTowerState(PlayerInputSystem o) { this.o = o; }

            public void Enter()
            {
                var towerUI = GM.Instance.TowerUISystem;

                o.ChoosedTower = o.hit.transform.gameObject;

                if (!towerUI.gameObject.activeSelf)
                    towerUI.gameObject.SetActive(true);

                o.StartCoroutine(towerUI.RefreshUI());

                if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
                    GM.PLAYERSTATE = GM.CHOOSED_TOWER;

                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class MouseNotOnUIState : IState
        {
            private readonly PlayerInputSystem o;

            public MouseNotOnUIState(PlayerInputSystem o) { this.o = o; }

            public void Enter()
            {
                if (GM.Instance.TowerUISystem.gameObject.activeSelf)
                    GM.Instance.TowerUISystem.gameObject.SetActive(false);
                
                if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
                    GM.PLAYERSTATE = GM.IDLE;

                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class SellTowerState : IState
        {
            private readonly PlayerInputSystem o;

            public SellTowerState(PlayerInputSystem o) { this.o = o; }

            public void Enter()
            {
                o.ChoosedTower.GetComponent<Tower.TowerBaseSystem>().Sell();
                o.state.ChangeState(new GetInputState(o));
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
            private readonly PlayerInputSystem o;

            public UpgradeTowerState(PlayerInputSystem o) { this.o = o; }

            public void Enter()
            {
                o.ChoosedTower.GetComponent<Tower.TowerBaseSystem>().Upgrade();
                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.TowerUISystem.IsUpgrading = false;                
            }
        }

        protected class CreateNewTowerState : IState
        {
            private readonly PlayerInputSystem o;

            public CreateNewTowerState(PlayerInputSystem o) { this.o = o; }

            public void Enter()
            {               
                GM.PLAYERSTATE = GM.PREPARE_PLACING_TOWER;

                for (int i = 0; i < GM.Instance.AvailableTowerList.Count; i++)
                {
                    if (GM.Instance.AvailableTowerList[i] == o.NewTowerData)
                    {
                        GM.Instance.AvailableTowerList.RemoveAt(i);
                        break;
                    }
                }

                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.ElementId);

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
