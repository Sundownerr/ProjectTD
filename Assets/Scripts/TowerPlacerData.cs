using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerPlacer
{
    public class TowerPlacerData : MonoBehaviour
    {
        public GameObject Tower, UIManager, GridManager;       
        public Vector3 TowerPosition;
        public Color TowerColor;

        private List<GameObject> buildingCubes;
        private List<BuildingCube> cubeStates;
        private TowerPlacerFSM FSM;
        private bool canBuild, towerCreated;       
        private CreateGrid grid;
        private UI ui;
       
        private IEnumerator GetBuildingCubesData()
        {
            while (!grid.isGridBuilded)
                yield return new WaitForSeconds(0.05f);

            buildingCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("BuildingCube"));
            cubeStates = new List<BuildingCube>();

            for (int i = 0; i < buildingCubes.Count; i++)
                cubeStates.Add(buildingCubes[i].GetComponent<BuildingCube>());

            FSM = new TowerPlacerFSM(new CreateTowerState(), Tower,  buildingCubes, cubeStates, ui);

            canBuild = true;           
        }

        private void Start()
        {
            GridManager = GameObject.Find("GridManager");
            UIManager = GameObject.Find("UIManager");
            grid = GridManager.GetComponent<CreateGrid>();
            ui = UIManager.GetComponent<UI>();        
               
            StartCoroutine(GetBuildingCubesData());
        }

        private void Update()
        {
            if (ui.BuildModeActive && canBuild)
            {
                if (!towerCreated)
                {
                    FSM.CreateTower();
                    towerCreated = true;
                }

                FSM.PlaceTower();
                TowerPosition = FSM.TowerPosition;
                TowerColor = FSM.TowerColor;
            }
            else
                towerCreated = false;
        }
    }

    interface IState
    {
        void CreateTower(TowerPlacerFSM FSM);
        void PlaceTower(TowerPlacerFSM FSM);  
    }

    class TowerPlacerFSM
    {
        public List<GameObject> TowerList, BuildingCubes;
        public List<BuildingCube> CubeStates;
        public IState State { get; set; }
        public GameObject towerPrefab;
        public Vector3 TowerPosition;
        public Color TowerColor;
        public UI Ui;
   
        public TowerPlacerFSM(IState state, GameObject towerPrefab, List<GameObject> buildingCubes, List<BuildingCube> cubeStates, UI ui)
        {
            TowerList = new List<GameObject>();
            this.towerPrefab = towerPrefab;
            BuildingCubes = buildingCubes;
            CubeStates = cubeStates;
            State = state;           
            Ui = ui;        
        }

        public void CreateTower()
        {
            State.CreateTower(this);
        }

        public void PlaceTower()
        {
            State.PlaceTower(this);
        }    
    }

    class CreateTowerState : IState
    {
        public void CreateTower(TowerPlacerFSM FSM)
        {           
            FSM.TowerList.Add(Object.Instantiate(FSM.towerPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f)));
            FSM.State = new PlaceTowerState();           
        }

        public void PlaceTower(TowerPlacerFSM FSM)
        {  
        }
    }

    class PlaceTowerState : IState
    {
        public void CreateTower(TowerPlacerFSM FSM)
        {    
        }

        public void PlaceTower(TowerPlacerFSM FSM)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit();
            

            if (Physics.Raycast(ray, out hit))
            {
                FSM.TowerPosition = hit.point;
                FSM.TowerColor = Color.red - new Color(0, 0, 0, 0.6f);
                
                for (int i = 0; i < FSM.BuildingCubes.Count; i++)
                {                   
                    if (hit.transform.gameObject == FSM.BuildingCubes[i] && !FSM.CubeStates[i].isBusy)
                    {
                        FSM.TowerPosition = FSM.BuildingCubes[i].transform.position;
                        FSM.TowerColor = Color.green - new Color(0, 0, 0, 0.6f);
                        FSM.CubeStates[i].isChosen = true;

                        if (Input.GetMouseButtonDown(0))
                        {                            
                            FSM.CubeStates[i].isBusy = true;
                            FSM.Ui.BuildModeActive = false;

                            FSM.State = new CreateTowerState();
                        }
                    }
                    else
                        FSM.CubeStates[i].isChosen = false;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    var lastTowerIndex = FSM.TowerList.Count - 1;

                    Object.Destroy(FSM.TowerList[lastTowerIndex]);
                    FSM.TowerList.RemoveAt(lastTowerIndex);
                    FSM.Ui.BuildModeActive = false;

                    FSM.State = new CreateTowerState();
                }
            }          
        } 
    }   
}