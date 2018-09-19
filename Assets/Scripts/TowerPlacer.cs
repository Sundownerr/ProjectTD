using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerPlacer : MonoBehaviour
{
    public GameObject Tower, UIManager, GridManager;
    public Vector3 TowerPosition;
    public Color TowerColor;

    private List<GameObject> buildingCubes;
    private List<BuildingCube> cubeStates;

    private bool canBuild, towerCreated;
    private CreateGrid grid;
    private UI ui;
    private RaycastHit hit;
    private List<GameObject> towerList;

    private IEnumerator GetBuildingCubesData()
    {
        while (!grid.isGridBuilded)
        {
            yield return new WaitForSeconds(0.05f);
        }

        buildingCubes = new List<GameObject>(GameObject.FindGameObjectsWithTag("BuildingCube"));
        cubeStates = new List<BuildingCube>();

        for (int i = 0; i < buildingCubes.Count; i++)
        {
            cubeStates.Add(buildingCubes[i].GetComponent<BuildingCube>());
        }

        canBuild = true;
    }

    private void CreateTower()
    {
        towerList.Add(Instantiate(Tower, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f)));
    }

    private void PlaceTower()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            TowerPosition = hit.point;
            TowerColor = Color.red - new Color(0, 0, 0, 0.6f);

            for (int i = 0; i < buildingCubes.Count; i++)
            {
                if (hit.transform.gameObject == buildingCubes[i] && !cubeStates[i].isBusy)
                {
                    TowerPosition = buildingCubes[i].transform.position;
                    TowerColor = Color.green - new Color(0, 0, 0, 0.6f);
                    cubeStates[i].isChosen = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        cubeStates[i].isBusy = true;
                        ui.BuildModeActive = false;
                    }
                }
                else
                {
                    cubeStates[i].isChosen = false;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                DeleteTower();
            }
        }
    }

    private void DeleteTower()
    {
        var lastTowerIndex = towerList.Count - 1;

        Destroy(towerList[lastTowerIndex]);
        towerList.RemoveAt(lastTowerIndex);

        ui.BuildModeActive = false;
    }

    private void Start()
    {
        GridManager = GameObject.Find("GridManager");
        UIManager = GameObject.Find("UIManager");
        grid = GridManager.GetComponent<CreateGrid>();
        ui = UIManager.GetComponent<UI>();
        towerList = new List<GameObject>();

        StartCoroutine(GetBuildingCubesData());
    }

    private void Update()
    {
        if (ui.BuildModeActive && canBuild)
        {
            if (!towerCreated)
            {
                CreateTower();
                towerCreated = true;
            }

            PlaceTower();
        }
        else
            towerCreated = false;
    }
}
