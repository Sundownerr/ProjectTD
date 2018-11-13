using System;
using System.Collections;
using Game.Tower.Data.Stats;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Systems
{
    public class TowerPlaceSystem
    {
        public GameObject NewBusyCell { get => newBusyCell; set => newBusyCell = value; }
        public Vector3 GhostedTowerPos { get => ghostedTowerPos; set => ghostedTowerPos = value; }
        public Color GhostedTowerColor { get => ghostedTowerColor; set => ghostedTowerColor = value; }
        public bool IsTowerLimitOk { get => isTowerLimitOk; set => isTowerLimitOk = value; }
        public event EventHandler TowerStateChanged = delegate{};

        private bool isTowerLimitOk;
        private Color transparentRed, transparentGreen, towerColor, ghostedTowerColor;
        private GameObject newBusyCell;
        private Tower.TowerSystem lastTower;
        private RaycastHit hit;
        private Camera mainCam;
        private StateMachine state;
        private Vector3 towerPos, ghostedTowerPos;
        private Cells.Cell chosenCell;
        private int newTowerLimit, newGoldCost, newMagicCrystalCost;

        public TowerPlaceSystem()
        {
            GM.I.TowerPlaceSystem = this;
            mainCam = Camera.main;

            transparentRed      = Color.red - new Color(0, 0, 0, 0.8f);
            transparentGreen    = Color.green - new Color(0, 0, 0, 0.8f);        

            state = new StateMachine();
            state.ChangeState(new GetCellDataState(this));
        }

        public void Update() => state.Update();
  
        protected class GetCellDataState : IState
        {
            private readonly TowerPlaceSystem o;

            public GetCellDataState(TowerPlaceSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute()
            {
                if (GM.I.GridSystem.IsGridBuilded)
                    o.state.ChangeState(new GetInputState(o));
            }

            public void Exit() { }
        }

        protected class GetInputState : IState
        {
            private readonly TowerPlaceSystem o;

            public GetInputState(TowerPlaceSystem o) => this.o = o; 

            public void Enter() => GM.PlayerState = State.Idle;

            public void Execute()
            {                
                if (GM.PlayerState == State.PreparePlacingTower)
                {
                    o.newTowerLimit         = GM.I.PlayerInputSystem.NewTowerData.TowerLimit;
                    o.newGoldCost           = GM.I.PlayerInputSystem.NewTowerData.GoldCost;
                    o.newMagicCrystalCost   = GM.I.PlayerInputSystem.NewTowerData.MagicCrystalReq;
                    
                    if (GM.I.ResourceSystem.CheckHaveResources(o.newTowerLimit, o.newGoldCost, o.newMagicCrystalCost))
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
                CreateTower();     
                o.state.ChangeState(new MoveTowerState(o));

                void CreateTower()
                {                
                    GM.PlayerState = State.PlacingTower;
                    GM.I.PlacedTowerList.Add(U.Instantiate(
                        GM.I.PlayerInputSystem.NewTowerData.Prefab, 
                        Vector3.zero - Vector3.up * 10, 
                        Quaternion.identity, 
                        GM.I.TowerParent));  

                    o.lastTower = GM.I.PlacedTowerList[GM.I.PlacedTowerList.Count - 1].GetComponent<Tower.TowerSystem>();
                    o.lastTower.Stats = U.Instantiate(GM.I.PlayerInputSystem.NewTowerData);
                    o.lastTower.Stats.IsInstanced = true;
                    o.lastTower.SetSystem();
                    GM.I.ResourceSystem.AddTowerLimit(o.newTowerLimit);
                    GM.I.ResourceSystem.AddGold(-o.newGoldCost);

                    o.TowerStateChanged?.Invoke(o, new EventArgs());                       
                }
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
                MoveTower();      

                void MoveTower()
                {          
                    var ray = o.mainCam.ScreenPointToRay(Input.mousePosition);

                    var cellList    = GM.I.CellStateList;
                    var cellGOList  = GM.I.CellList;

                    var terrainLayer    = 1 << 9;
                    var cellLayer       = 1 << 15;
                    var layerMask       = terrainLayer | cellLayer;

                    if (Input.GetMouseButtonDown(1))
                            o.state.ChangeState(new DeleteTowerState(o));

                    if (Physics.Raycast(ray, out o.hit, 5000, layerMask))
                    {              
                        o.towerPos = o.hit.point;
                        o.towerColor = o.transparentRed;             

                        for (int i = 0; i < cellGOList.Count; i++)
                        {                   
                            var isHitCell           = o.hit.transform.gameObject == cellGOList[i];
                            var isHitCellNotBusy    = isHitCell && !cellList[i].IsBusy;
                            cellList[i].IsChosen = isHitCellNotBusy;
                        
                            if(isHitCellNotBusy)
                            {
                                o.chosenCell = cellList[i];
            
                                o.towerPos = o.chosenCell.transform.position;
                                o.towerColor = o.transparentGreen;                   

                                if (Input.GetMouseButtonDown(0))
                                    o.state.ChangeState(new PlaceTowerState(o));
                            } 
                        }           

                        SetTowerColorAndPosition();
                    }

                    void SetTowerColorAndPosition()
                    {
                        o.GhostedTowerPos     = o.towerPos;
                        o.GhostedTowerColor   = o.towerColor;
                    }
                }     
            }

            public void Exit() { }
        }

        protected class PlaceTowerState : IState
        {
            private readonly TowerPlaceSystem o;

            public PlaceTowerState(TowerPlaceSystem o) => this.o = o; 

            public void Enter()
            {
                PlaceTower();
                o.TowerStateChanged?.Invoke(o, new EventArgs());

                o.state.ChangeState(new GetInputState(o));      

                void PlaceTower()
                {
                    o.lastTower.OcuppiedCell = o.chosenCell.gameObject;
                    o.chosenCell.IsBusy = true;              
                }
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
                DeleteTower();
                o.state.ChangeState(new GetInputState(o));

                void DeleteTower()
                {         
                    var lastTower = GM.I.PlacedTowerList[GM.I.PlacedTowerList.Count - 1];

                    GM.I.ResourceSystem.AddTowerLimit(-o.newTowerLimit);
                    GM.I.ResourceSystem.AddGold(o.newGoldCost);

                    U.Destroy(lastTower);
                    GM.I.PlacedTowerList.Remove(lastTower);
                    
                    GM.I.BuildUISystem.AddTowerButton(GM.I.PlayerInputSystem.NewTowerData); 
                    GM.I.AvailableTowerList.Add(GM.I.PlayerInputSystem.NewTowerData);

                    o.TowerStateChanged?.Invoke(o, new EventArgs());   
                }
            }

            public void Execute() { }

            public void Exit() { }
        }
    }  
}