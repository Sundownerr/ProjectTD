using System.Collections;
using UnityEngine;

namespace Game.System
{
    public class TowerPlaceSystem : ExtendedMonoBehaviour
    { 
        [HideInInspector]
        public Vector3 GhostedTowerPos;

        [HideInInspector]
        public Color GhostedTowerColor;

        [HideInInspector]
        public GameObject NewBusyCell;

        public LayerMask LayerMask;
        
        private Color transparentRed, transparentGreen, towerColor;     
        private RaycastHit hit;
        private Camera mainCam;
        private StateMachine state;
        private Vector3 towerPos;
       
        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GameManager.Instance.TowerPlaceSystem = this;
            mainCam = Camera.main;

            transparentRed = Color.red - new Color(0, 0, 0, 0.8f);
            transparentGreen = Color.green - new Color(0, 0, 0, 0.8f);

            state = new StateMachine();
            state.ChangeState(new GetCellDataState(this));
        }

        private void Update()
        {
            state.Update();
        }

        private IEnumerator Refresh()
        {
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;

            yield return new WaitForFixedUpdate();

            state.ChangeState(new CreateTowerState(this));            
        }

        private void CreateGhostedTower()
        {
            GameManager.Instance.TowerList.Add(Instantiate(GameManager.Instance.TowerPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), GameManager.Instance.TowerParent));
        }

        private void SetTowerColorAndPosition(Vector3 pos, Color color)
        {
            GhostedTowerPos = pos;
            GhostedTowerColor = color;
        }

        private void MoveGhostedTower()
        {          
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var cellStateList = GameManager.Instance.CellStateList;
            var cellList = GameManager.Instance.CellList;

            if (Physics.Raycast(ray, out hit, 5000, LayerMask))
            {              
                towerPos = hit.point;
                towerColor = transparentRed;

                for (int i = 0; i < cellList.Count; i++)
                {                   
                    var isHitCell = hit.transform.gameObject == cellList[i];                 

                    if (isHitCell && !cellStateList[i].IsBusy)
                    {
                        towerPos = cellStateList[i].transform.position;
                        towerColor = transparentGreen;

                        cellStateList[i].IsChosen = true;

                        if (Input.GetMouseButtonDown(0))
                        {
                            NewBusyCell = cellList[i];
                            cellStateList[i].IsBusy = true;                            

                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                StartCoroutine(Refresh());                                
                            }
                            else
                            {
                                state.ChangeState(new GetInputState(this));                               
                            }
                        }
                    }
                    else
                    {
                        cellStateList[i].IsChosen = false;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    DeleteTower();
                }

                SetTowerColorAndPosition(towerPos, towerColor);
            }
        }

        private void DeleteTower()
        {
            var towerList = GameManager.Instance.TowerList;
            var lastTowerIndex = towerList.Count - 1;

            Destroy(towerList[lastTowerIndex]);
            towerList.RemoveAt(lastTowerIndex);

            state.ChangeState(new GetInputState(this));      
        }

        public class GetCellDataState : IState
        {
            TowerPlaceSystem owner;

            public GetCellDataState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {

            }

            public void Execute()
            {
                if (GameManager.Instance.GridSystem.IsGridBuilded)
                {
                    owner.state.ChangeState(new GetInputState(owner));
                }
            }

            public void Exit()
            {

            }
        }

        public class GetInputState : IState
        {
            TowerPlaceSystem owner;

            public GetInputState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
            }

            public void Execute()
            {
                if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    owner.state.ChangeState(new CreateTowerState(owner));                   
                }
            }

            public void Exit()
            {

            }
        }

        public class CreateTowerState : IState
        {
            TowerPlaceSystem owner;

            public CreateTowerState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
                owner.CreateGhostedTower();
                owner.state.ChangeState(new PlaceTowerState(owner));               
            }

            public void Execute()
            {

            }

            public void Exit()
            {

            }
        }


        public class PlaceTowerState : IState
        {
            TowerPlaceSystem owner;

            public PlaceTowerState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                
            }

            public void Execute()
            {
                owner.MoveGhostedTower();
            }

            public void Exit()
            {

            }
        }
    }

    
}