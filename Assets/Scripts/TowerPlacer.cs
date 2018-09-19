using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TowerPlacer : MonoBehaviour
{
    public GameObject Tower, UIManager, GridManager;
    public Vector3 GhostedTowerPosition;
    public Color GhostedTowerColor;

    private List<GameObject> buildingCubeList, towerList;
    private List<BuildingCube> cubeStateList;
    private bool canBuild, isTowerCreated;
    private CreateGrid grid;
    private UI ui;
    private RaycastHit hit;
    

    private IEnumerator GetBuildingCubesData()
    {
        while (!grid.IsGridBuilded)
        {
            yield return new WaitForSeconds(0.05f);
        }

        buildingCubeList = new List<GameObject>(GameObject.FindGameObjectsWithTag("BuildingCube"));
        cubeStateList = new List<BuildingCube>();

        for (int i = 0; i < buildingCubeList.Count; i++)
        {
            cubeStateList.Add(buildingCubeList[i].GetComponent<BuildingCube>());
        }

        canBuild = true;
    }

    private void CreateGhostedTower()
    {
        towerList.Add(Instantiate(Tower, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f)));
    }

    private void PlaceGhostedTower()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GhostedTowerPosition = hit.point;
            GhostedTowerColor = Color.red - new Color(0, 0, 0, 0.6f);

            for (int i = 0; i < buildingCubeList.Count; i++)
            {
                if (hit.transform.gameObject == buildingCubeList[i] && !cubeStateList[i].IsBusy)
                {
                    GhostedTowerPosition = buildingCubeList[i].transform.position;
                    GhostedTowerColor = Color.green - new Color(0, 0, 0, 0.6f);
                    cubeStateList[i].IsChosen = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        cubeStateList[i].IsBusy = true;
                        ui.IsBuildModeActive = false;
                    }
                }
                else
                {
                    cubeStateList[i].IsChosen = false;
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

        ui.IsBuildModeActive = false;
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
        if (ui.IsBuildModeActive && canBuild)
        {
            if (!isTowerCreated)
            {
                CreateGhostedTower();
                isTowerCreated = true;
            }

            PlaceGhostedTower();
        }
        else
            isTowerCreated = false;
    }
}
