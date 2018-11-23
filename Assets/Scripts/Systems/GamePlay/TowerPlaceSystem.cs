using System;
using System.Collections;
using Game.Tower;
using Game.Tower.Data;
using Game.Tower.Data.Stats;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Systems
{
    public class TowerEventArgs
    {
     
        public TowerData Stats;
        public TowerSystem System;


        public TowerEventArgs(TowerSystem system, TowerData stats) : base()
        {
            Stats = stats;
            System = system;         
        }

        public TowerEventArgs(TowerSystem tower) : base() => System = tower;        
        
        public TowerEventArgs(TowerData stats) : base() => Stats = stats;               
    }

    public class TowerPlaceSystem
    {
        public GameObject NewBusyCell { get => newBusyCell; set => newBusyCell = value; }      
        public bool IsTowerLimitOk { get => isTowerLimitOk; set => isTowerLimitOk = value; }
        public event EventHandler TowerStateChanged = delegate{};
        public event EventHandler<TowerEventArgs> TowerDeleted = delegate{};
        public event EventHandler<TowerEventArgs> TowerCreated = delegate{};
        public event EventHandler<TowerEventArgs> TowerPlaced = delegate{};

        private bool isTowerLimitOk;
        private Color transparentRed, transparentGreen;
        private GameObject newBusyCell;
        private Tower.TowerSystem lastTower;
        private RaycastHit hit;
        private Camera mainCam;
        private Cells.Cell chosenCell;
        private int newTowerLimit, newGoldCost, newMagicCrystalCost;
  
        public TowerPlaceSystem()
        {
            GM.I.TowerPlaceSystem = this;
            mainCam = Camera.main;

            transparentRed      = Color.red - new Color(0, 0, 0, 0.8f);
            transparentGreen    = Color.green - new Color(0, 0, 0, 0.8f);        
        }

        public void UpdateSystem() 
        {
            if (GM.I.GridSystem.IsGridBuilded)          
                if (GM.PlayerState == State.PreparePlacingTower)
                {
                    newTowerLimit         = GM.I.PlayerInputSystem.NewTowerData.TowerLimit;
                    newGoldCost           = GM.I.PlayerInputSystem.NewTowerData.GoldCost;
                    newMagicCrystalCost   = GM.I.PlayerInputSystem.NewTowerData.MagicCrystalReq;
                    
                    if (GM.I.ResourceSystem.CheckHaveResources(newTowerLimit, newGoldCost, newMagicCrystalCost))
                        CreateTower();                    
                }
                else
                    if(GM.PlayerState == State.PlacingTower)
                        MoveTower();                       
  
            void CreateTower()
            {                
                GM.PlayerState = State.PlacingTower;

                var lastTowerPrefab = U.Instantiate(
                    GM.I.PlayerInputSystem.NewTowerData.Prefab, 
                    Vector3.zero - Vector3.up * 10, 
                    Quaternion.identity, 
                    GM.I.TowerParent);                 

                lastTower = new TowerSystem(lastTowerPrefab);
                lastTower.Stats = GM.I.PlayerInputSystem.NewTowerData;                   
                lastTower.SetSystem();
                GM.I.PlacedTowerList.Add(lastTower);  
                
                TowerCreated?.Invoke(this, new TowerEventArgs(lastTower, lastTower.Stats));
                TowerStateChanged?.Invoke(this, new EventArgs());                       
            }

            void MoveTower()
            {          
                var ray = mainCam.ScreenPointToRay(Input.mousePosition);
                var terrainLayer    = 1 << 9;
                var cellLayer       = 1 << 15;
                var layerMask       = terrainLayer | cellLayer;

                if (Input.GetMouseButtonDown(1))
                    DeleteTower();

                if (Physics.Raycast(ray, out hit, 5000, layerMask))
                {              
                    lastTower.Prefab.transform.position = hit.point;
                    SetTowerColor(lastTower, transparentRed);   
                    
                    if(hit.transform.gameObject.layer == 15)
                    {
                        var cell = GM.I.GridSystem.CellList.Find(hitCell => hitCell.gameObject == hit.transform.gameObject);          
                    
                        if(!cell.IsBusy)
                        {
                            chosenCell = cell;
        
                            lastTower.Prefab.transform.position = chosenCell.transform.position;
                            SetTowerColor(lastTower, transparentGreen);                 

                            if (Input.GetMouseButtonDown(0))
                                PlaceTower();
                        }   
                    }                                            
                }                  
            }

            void PlaceTower()
            {                  
                lastTower.OcuppiedCell = chosenCell.gameObject;                       
                lastTower.Prefab.transform.position = lastTower.OcuppiedCell.transform.position;          

                var placeEffect = U.Instantiate(GM.I.ElementPlaceEffectList[(int)lastTower.Stats.Element],
                    lastTower.Prefab.transform.position + Vector3.up * 5,
                    Quaternion.identity);
                U.Destroy(placeEffect, placeEffect.GetComponent<ParticleSystem>().main.duration);

                SetTowerColor(lastTower, Color.white - new Color(0.2f, 0.2f, 0.2f));                  
                  
                GM.PlayerState = State.Idle;
                TowerPlaced?.Invoke(this, new TowerEventArgs(lastTower));
                TowerStateChanged?.Invoke(this, new EventArgs());  
            }     

            void DeleteTower()
            {         
                GM.PlayerState = State.Idle;
                TowerDeleted?.Invoke(this, new TowerEventArgs(lastTower, lastTower.Stats));
                TowerStateChanged?.Invoke(this, new EventArgs());   
            }

            void SetTowerColor(TowerSystem tower, Color color)
            {
                for (int i = 0; i < tower.RendererList.Length; i++)
                    tower.RendererList[i].material.color = color;
            }
        }   
    }  
}