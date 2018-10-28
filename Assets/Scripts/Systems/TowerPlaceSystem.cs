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
        public bool IsTowerLimitOk;
        
        private Color transparentRed, transparentGreen, towerColor;
        private GameObject lastTower;
        private RaycastHit hit;
        private Camera mainCam;
        private StateMachine state;
        private Vector3 towerPos;
        private Cells.Cell chosenCellState;
        private int newTowerLimit, newGoldCost, newMagicCrystalCost;
       
        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }            
            mainCam = Camera.main;

            transparentRed = Color.red - new Color(0, 0, 0, 0.8f);
            transparentGreen = Color.green - new Color(0, 0, 0, 0.8f);

            GM.Instance.TowerPlaceSystem = this;

            state = new StateMachine();
            state.ChangeState(new GetCellDataState(this));
        }

        private void Update()
        {
            state.Update();
        }

        private IEnumerator Refresh()
        {
            GM.PLAYERSTATE = GM.IDLE;

            yield return new WaitForFixedUpdate();

            state.ChangeState(new CreateTowerState(this));            
        }

        private void CreateTower()
        {
            if (GM.Instance.ResourceSystem.CheckHaveResources(newTowerLimit, newGoldCost, newMagicCrystalCost))
            {
                GM.PLAYERSTATE = GM.PLACING_TOWER;

                var newTower = Instantiate(GM.Instance.PlayerInputSystem.NewTowerData.Prefab, Vector3.zero - Vector3.up * 10, Quaternion.identity, GM.Instance.TowerParent);        
                newTower.GetComponent<Tower.TowerBaseSystem>().StatsSystem.Stats = GM.Instance.PlayerInputSystem.NewTowerData;
                newTower.GetComponent<Tower.TowerBaseSystem>().SetSystem();

                GM.Instance.PlacedTowerList.Add(newTower);
               

                GM.Instance.ResourceSystem.AddTowerLimit(newTowerLimit);
                GM.Instance.ResourceSystem.AddGold(-newGoldCost);

                lastTower = GM.Instance.PlacedTowerList[GM.Instance.PlacedTowerList.Count - 1];

                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.ElementId);

                state.ChangeState(new MoveTowerState(this));
            }
            else
            {
                state.ChangeState(new GetInputState(this));
            }
        }

        private void SetTowerColorAndPosition(Vector3 pos, Color color)
        {
            GhostedTowerPos = pos;
            GhostedTowerColor = color;
        }

        private void MoveTower()
        {          
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var cellStateList = GM.Instance.CellStateList;
            var cellList = GM.Instance.CellList;

            if (Physics.Raycast(ray, out hit, 5000, LayerMask))
            {              
                towerPos = hit.point;
                towerColor = transparentRed;

                for (int i = 0; i < cellList.Count; i++)
                {                   
                    var isHitCell = hit.transform.gameObject == cellList[i];                 

                    if (isHitCell && !cellStateList[i].IsBusy)
                    {
                        chosenCellState = cellStateList[i];
                        chosenCellState.IsChosen = true;

                        towerPos = chosenCellState.transform.position;
                        towerColor = transparentGreen;                   

                        if (Input.GetMouseButtonDown(0))
                        {
                            state.ChangeState(new PlaceTowerState(this));
                        }
                    }
                    else
                    {
                        cellStateList[i].IsChosen = false;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    state.ChangeState(new DeleteTowerState(this));
                }

                SetTowerColorAndPosition(towerPos, towerColor);
            }
        }

        private void PlaceTower()
        {
            lastTower.GetComponent<Tower.TowerBaseSystem>().OcuppiedCell = chosenCellState.gameObject;
            chosenCellState.IsBusy = true;
        
            state.ChangeState(new GetInputState(this));            
        }

        private void DeleteTower()
        {         
            var lastTowerIndex = GM.Instance.PlacedTowerList.Count - 1;

            GM.Instance.ResourceSystem.AddTowerLimit(-newTowerLimit);
            GM.Instance.ResourceSystem.AddGold(newGoldCost);

            Destroy(GM.Instance.PlacedTowerList[lastTowerIndex]);
            GM.Instance.PlacedTowerList.RemoveAt(lastTowerIndex);

            GM.Instance.AvailableTowerList.Add(GM.Instance.PlayerInputSystem.NewTowerData);
            GM.Instance.BuildUISystem.UpdateAvailableElement();
            GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.ElementId);

            state.ChangeState(new GetInputState(this));
        }

        protected class GetCellDataState : IState
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
                if (GM.Instance.GridSystem.IsGridBuilded)
                {
                    owner.state.ChangeState(new GetInputState(owner));
                }
            }

            public void Exit()
            {

            }
        }

        protected class GetInputState : IState
        {
            TowerPlaceSystem owner;

            public GetInputState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                GM.PLAYERSTATE = GM.IDLE;
            }

            public void Execute()
            {                
                if (GM.PLAYERSTATE == GM.PREPARE_PLACING_TOWER)
                {
                    owner.newTowerLimit = GM.Instance.PlayerInputSystem.NewTowerData.TowerLimit;
                    owner.newGoldCost = GM.Instance.PlayerInputSystem.NewTowerData.GoldCost;
                    owner.newMagicCrystalCost = GM.Instance.PlayerInputSystem.NewTowerData.MagicCrystalReq;
                    
                    if (GM.Instance.ResourceSystem.CheckHaveResources(owner.newTowerLimit, owner.newGoldCost, owner.newMagicCrystalCost))
                    {
                        owner.state.ChangeState(new CreateTowerState(owner));
                    }
                    else
                    {
                        owner.state.ChangeState(new GetInputState(owner));
                    }
                }
            }

            public void Exit()
            {

            }
        }

        protected class CreateTowerState : IState
        {
            TowerPlaceSystem owner;

            public CreateTowerState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.CreateTower();      
            }

            public void Execute()
            {

            }

            public void Exit()
            {

            }
        }


        protected class MoveTowerState : IState
        {
            TowerPlaceSystem owner;

            public MoveTowerState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                
            }

            public void Execute()
            {
                owner.MoveTower();
            }

            public void Exit()
            {

            }
        }

        protected class PlaceTowerState : IState
        {
            TowerPlaceSystem owner;

            public PlaceTowerState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.PlaceTower();
                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.ElementId);
            }

            public void Execute()
            {
            }

            public void Exit()
            {

            }
        }

        protected class DeleteTowerState : IState
        {
            TowerPlaceSystem owner;

            public DeleteTowerState(TowerPlaceSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.DeleteTower();
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