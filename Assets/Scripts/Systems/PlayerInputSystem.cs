using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#pragma warning disable CS1591 
namespace Game.System
{
    public class PlayerInputSystem : MonoBehaviour
    {
        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;
        public GameObject ChoosedTower;     
        public LayerMask LayerMask;

        private PointerEventData pointerEventData;
        private List<RaycastResult> results;
        private RaycastHit hit;
        private Ray WorldRay;
        private bool isHitUI;
        private StateMachine state;

        private void Start()
        {
            GameManager.Instance.PlayerInputSystem = this;
            results = new List<RaycastResult>();


            state = new StateMachine();
            state.ChangeState(new GetInputState(this));
        }

        private void Update()
        {
            state.Update();
        }

        public class GetInputState : IState
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
                owner.pointerEventData = new PointerEventData(owner.EventSystem)
                {
                    position = Input.mousePosition
                };

                if (Input.GetMouseButtonDown(0))
                {
                    owner.GraphicRaycaster.Raycast(owner.pointerEventData, owner.results);

                    if (owner.results.Count > 0)
                    {
                        owner.isHitUI = true;
                    }
                }

                owner.WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(owner.WorldRay, out owner.hit, 10000, owner.LayerMask))
                {
                    var isMouseOnTower = 
                        !owner.isHitUI && 
                        owner.hit.transform.gameObject.layer == 14;

                    var isMouseNotOnUI = 
                        !owner.isHitUI && 
                        owner.hit.transform.gameObject.layer == 9;

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (isMouseOnTower && !Input.GetKey(KeyCode.LeftShift))
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
            }

            public void Exit()
            {
            }
        }

        public class MouseOnTowerState : IState
        {
            private readonly PlayerInputSystem owner;

            public MouseOnTowerState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                if (owner.ChoosedTower != owner.hit.transform.gameObject)
                {
                    owner.ChoosedTower = owner.hit.transform.gameObject;

                    if (!GameManager.Instance.TowerUISystem.gameObject.activeSelf)
                    {
                        GameManager.Instance.TowerUISystem.gameObject.SetActive(true);
                    }

                    owner.StartCoroutine(GameManager.Instance.TowerUISystem.RefreshUI());

                    if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
                    {
                        GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_CHOOSEDTOWER;
                    }
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

        public class MouseNotOnUIState : IState
        {
            private readonly PlayerInputSystem owner;

            public MouseNotOnUIState(PlayerInputSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                if (GameManager.Instance.TowerUISystem.gameObject.activeSelf)
                {
                    GameManager.Instance.TowerUISystem.gameObject.SetActive(false);
                }

                if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
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
    }
}
//pointerEventData = new PointerEventData(EventSystem)
//{
//    position = Input.mousePosition
//            };

//            if (Input.GetMouseButtonDown(0))
//            {
//                GraphicRaycaster.Raycast(pointerEventData, results);

//                if (results.Count > 0)
//                {
//                    isHitUI = true;
//                }
//            }            

//            WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);

//            if (Physics.Raycast(WorldRay, out hit, 10000, LayerMask))
//            {
//                var isMouseOnTower = !isHitUI && hit.transform.gameObject.layer == 14;
//var isMouseNotOnUI = !isHitUI && hit.transform.gameObject.layer == 9;

//                if (Input.GetMouseButtonDown(0))
//                {
//                    if (isMouseOnTower && !Input.GetKey(KeyCode.LeftShift))
//                    {
//                        ChoosedTower = hit.transform.gameObject;

//                        if (!GameManager.Instance.TowerUISystem.gameObject.activeSelf)
//                        {
//                            GameManager.Instance.TowerUISystem.gameObject.SetActive(true);
//                        }

//                        StartCoroutine(GameManager.Instance.TowerUISystem.RefreshUI());

//                        if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
//                        {
//                            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_CHOOSEDTOWER;
//                        }
//                    }

//                    if (isMouseNotOnUI)
//                    {
//                        if (GameManager.Instance.TowerUISystem.gameObject.activeSelf)
//                        {
//                            GameManager.Instance.TowerUISystem.gameObject.SetActive(false);
//                        }

//                        if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
//                        {
//                            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
//                        }
//                    }
//                }
//            }

//            if (results.Count > 0)
//            {
//                results.Clear();
//                isHitUI = false;
//            }