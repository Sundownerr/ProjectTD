using System.Collections;
using UnityEngine;

namespace Game.Systems
{
    public class TowerPlaceSystem
    {
        public GameObject NewBusyCell { get => newBusyCell; set => newBusyCell = value; }
        public Vector3 GhostedTowerPos { get => ghostedTowerPos; set => ghostedTowerPos = value; }
        public Color GhostedTowerColor { get => ghostedTowerColor; set => ghostedTowerColor = value; }
        public bool IsTowerLimitOk { get => isTowerLimitOk; set => isTowerLimitOk = value; }

        private bool isTowerLimitOk;
        private Color transparentRed, transparentGreen, towerColor, ghostedTowerColor;
        private GameObject newBusyCell;
        private Tower.TowerSystem lastTower;
        private RaycastHit hit;
        private Camera mainCam;
        private StateMachine state;
        private Vector3 towerPos, ghostedTowerPos;
        private Cells.Cell chosenCellState;
        private int newTowerLimit, newGoldCost, newMagicCrystalCost;

        public TowerPlaceSystem()
        {
            GM.Instance.TowerPlaceSystem = this;
            mainCam = Camera.main;

            transparentRed      = Color.red - new Color(0, 0, 0, 0.8f);
            transparentGreen    = Color.green - new Color(0, 0, 0, 0.8f);        

            state = new StateMachine();
            state.ChangeState(new GetCellDataState(this));
        }

        public void Update()
        {
            state.Update();
        }

        private IEnumerator Refresh()
        {
            GM.PlayerState = GM.State.Idle;

            yield return new WaitForFixedUpdate();

            state.ChangeState(new CreateTowerState(this));            
        }

        private void CreateTower()
        {
            var isHaveResources = GM.Instance.ResourceSystem.CheckHaveResources(newTowerLimit, newGoldCost, newMagicCrystalCost);

            if (!isHaveResources)
                state.ChangeState(new GetInputState(this));
            else
            {
                GM.PlayerState = GM.State.PlacingTower;

                var newTower = Object.Instantiate(
                    GM.Instance.PlayerInputSystem.NewTowerData.Prefab, 
                    Vector3.zero - Vector3.up * 10, Quaternion.identity, 
                    GM.Instance.TowerParent);  

                var newTowerSystem =  newTower.GetComponent<Tower.TowerSystem>();

                newTowerSystem.SetStats(Object.Instantiate(GM.Instance.PlayerInputSystem.NewTowerData));
                newTowerSystem.GetStats().IsInstanced = true;
                newTowerSystem.SetSystem();

                GM.Instance.PlacedTowerList.Add(newTower);              

                GM.Instance.ResourceSystem.AddTowerLimit(newTowerLimit);
                GM.Instance.ResourceSystem.AddGold(-newGoldCost);

                lastTower = newTowerSystem;

                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.Element);      
            }        
        }

        private void SetTowerColorAndPosition(Vector3 pos, Color color)
        {
            GhostedTowerPos     = pos;
            GhostedTowerColor   = color;
        }

        private void MoveTower()
        {          
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);

            var cellStateList   = GM.Instance.CellStateList;
            var cellList        = GM.Instance.CellList;

            var terrainLayer    = 1 << 9;
            var cellLayer       = 1 << 15;
            var layerMask       = terrainLayer | cellLayer;

            if (Input.GetMouseButtonDown(1))
                    state.ChangeState(new DeleteTowerState(this));

            if (Physics.Raycast(ray, out hit, 5000, layerMask))
            {              
                towerPos = hit.point;
                towerColor = transparentRed;             

                for (int i = 0; i < cellList.Count; i++)
                {                   
                    var isHitCell   = hit.transform.gameObject == cellList[i];
                    var isHitCellOk = isHitCell && !cellStateList[i].IsBusy;
                    cellStateList[i].IsChosen = isHitCellOk ? true : false;
                 
                    if(isHitCellOk)
                    {
                        chosenCellState = cellStateList[i];
     
                        towerPos = chosenCellState.transform.position;
                        towerColor = transparentGreen;                   

                        if (Input.GetMouseButtonDown(0))
                            state.ChangeState(new PlaceTowerState(this));
                    } 
                }           

                SetTowerColorAndPosition(towerPos, towerColor);
            }
        }

        private void PlaceTower()
        {
            lastTower.OcuppiedCell = chosenCellState.gameObject;
            chosenCellState.IsBusy = true;              
        }

        private void DeleteTower()
        {         
            var lastTowerIndex = GM.Instance.PlacedTowerList.Count - 1;

            GM.Instance.ResourceSystem.AddTowerLimit(-newTowerLimit);
            GM.Instance.ResourceSystem.AddGold(newGoldCost);

            Object.Destroy(GM.Instance.PlacedTowerList[lastTowerIndex]);
            GM.Instance.PlacedTowerList.RemoveAt(lastTowerIndex);

            GM.Instance.AvailableTowerList.Add(GM.Instance.PlayerInputSystem.NewTowerData);
            GM.Instance.BuildUISystem.UpdateAvailableElement();
            GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.Element);     
        }

        protected class GetCellDataState : IState
        {
            private readonly TowerPlaceSystem o;

            public GetCellDataState(TowerPlaceSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute()
            {
                if (GM.Instance.GridSystem.IsGridBuilded)
                    o.state.ChangeState(new GetInputState(o));
            }

            public void Exit() { }
        }

        protected class GetInputState : IState
        {
            private readonly TowerPlaceSystem o;

            public GetInputState(TowerPlaceSystem o) => this.o = o; 

            public void Enter() => GM.PlayerState = GM.State.Idle;

            public void Execute()
            {                
                if (GM.PlayerState == GM.State.PreparePlacingTower)
                {
                    o.newTowerLimit         = GM.Instance.PlayerInputSystem.NewTowerData.TowerLimit;
                    o.newGoldCost           = GM.Instance.PlayerInputSystem.NewTowerData.GoldCost;
                    o.newMagicCrystalCost   = GM.Instance.PlayerInputSystem.NewTowerData.MagicCrystalReq;

                    var isHaveResources = GM.Instance.ResourceSystem.CheckHaveResources(o.newTowerLimit, o.newGoldCost, o.newMagicCrystalCost);
                   
                    if (isHaveResources)
                        o.state.ChangeState(new CreateTowerState(o));
                    else
                        o.state.ChangeState(new GetInputState(o));
                }
            }

            public void Exit() { }
        }

        protected class CreateTowerState : IState
        {
            private readonly TowerPlaceSystem o;

            public CreateTowerState(TowerPlaceSystem o) => this.o = o; 

            public void Enter() 
            {
                o.CreateTower();     

                o.state.ChangeState(new MoveTowerState(o));
            }
            
            public void Execute() { }

            public void Exit() { }
        }

        protected class MoveTowerState : IState
        {
            private readonly TowerPlaceSystem o;

            public MoveTowerState(TowerPlaceSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute()  
            {
                o.MoveTower();           
            }

            public void Exit() { }
        }

        protected class PlaceTowerState : IState
        {
            private readonly TowerPlaceSystem o;

            public PlaceTowerState(TowerPlaceSystem o) => this.o = o; 

            public void Enter()
            {
                o.PlaceTower();
                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.PlayerInputSystem.NewTowerData.Element);

                o.state.ChangeState(new GetInputState(o));      
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class DeleteTowerState : IState
        {
            private readonly TowerPlaceSystem o;

            public DeleteTowerState(TowerPlaceSystem o) => this.o = o; 

            public void Enter()
            { 
                o.DeleteTower();
                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }
    }  
}