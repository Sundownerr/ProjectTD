using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.System
{
    public class PlayerInputSystem : ExtendedMonoBehaviour
    {
        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;
        public GameObject ChoosedTower;     
        public LayerMask LayerMask;

        private PointerEventData pointerEventData;
        private List<RaycastResult> results;
        private StateMachine state;
        private RaycastHit hit;
        private Ray WorldRay;
        private bool isHitUI;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }            
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

            public GetInputState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
            }

            public void Execute()
            {
                owner.WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                var isRayHit = Physics.Raycast(owner.WorldRay, out owner.hit, 10000, owner.LayerMask);

                owner.pointerEventData = new PointerEventData(owner.EventSystem);
                owner.pointerEventData.position = Input.mousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    owner.GraphicRaycaster.Raycast(owner.pointerEventData, owner.results);

                    if (owner.results.Count > 0)
                    {
                        owner.isHitUI = true;
                    }

                    if (isRayHit)
                    {
                        var isMouseOnTower =
                            !owner.isHitUI &&
                            owner.hit.transform.gameObject.layer == 14;

                        var isMouseNotOnUI =
                            !owner.isHitUI &&
                            owner.hit.transform.gameObject.layer == 9;

                        if (isMouseOnTower)
                        {
                            owner.state.ChangeState(new MouseOnTowerState(owner));
                        }

                        if (isMouseNotOnUI)
                        {
                            owner.state.ChangeState(new MouseNotOnUIState(owner));
                        }
                    }
                }

                if (owner.results.Count > 0)
                {
                    owner.results.Clear();
                    owner.isHitUI = false;
                }

                if(GM.Instance.TowerUISystem.IsSellig)
                {
                    owner.state.ChangeState(new SellTowerState(owner));
                }

                if (GM.Instance.TowerUISystem.IsUpgrading)
                {
                    owner.state.ChangeState(new UpgradeTowerState(owner));
                }
            }
            
            public void Exit()
            {
            }
        }

        protected class MouseOnTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public MouseOnTowerState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                var towerUI = GM.Instance.TowerUISystem;

                owner.ChoosedTower = owner.hit.transform.gameObject;

                if (!towerUI.gameObject.activeSelf)
                {
                    towerUI.gameObject.SetActive(true);
                }

                owner.StartCoroutine(towerUI.RefreshUI());

                if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
                {
                    GM.PLAYERSTATE = GM.CHOOSED_TOWER;
                }

                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute()
            {
            }

            public void Exit()
            {
            }
        }

        protected class MouseNotOnUIState : IState
        {
            private readonly PlayerInputSystem owner;

            public MouseNotOnUIState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                var towerUI = GM.Instance.TowerUISystem;

                if (towerUI.gameObject.activeSelf)
                {
                    towerUI.gameObject.SetActive(false);
                }
                
                if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
                {
                    GM.PLAYERSTATE = GM.IDLE;
                }

                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute()
            {
            }

            public void Exit()
            {
            }
        }

        protected class SellTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public SellTowerState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.ChoosedTower.GetComponent<Tower.TowerBaseSystem>().Sell();
                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute()
            {
            }

            public void Exit()
            {
                GM.Instance.TowerUISystem.IsSellig = false;
                GM.Instance.TowerUISystem.gameObject.SetActive(false);
            }
        }

        protected class UpgradeTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public UpgradeTowerState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.ChoosedTower.GetComponent<Tower.TowerBaseSystem>().Upgrade();
                owner.state.ChangeState(new GetInputState(owner));
            }

            public void Execute()
            {
            }

            public void Exit()
            {
                GM.Instance.TowerUISystem.IsUpgrading = false;                
            }
        }
    }
}
