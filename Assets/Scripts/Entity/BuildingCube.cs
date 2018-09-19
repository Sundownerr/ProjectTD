using System.Collections.Generic;
using UnityEngine;

public class BuildingCube : MonoBehaviour
{
    public GameObject Cube, UIManager, TowerPlacer;
    public bool IsBusy, IsChosen;

    private Color blueColor, redColor, greenColor;
    private List<GameObject> BuildingAreas;
    private Renderer cubeRenderer;
    private UI ui;   
    private bool isTransparent;

    private void Start ()
    {      
        UIManager = GameObject.Find("UIManager");
        BuildingAreas = new List<GameObject>(GameObject.FindGameObjectsWithTag("BuildingArea"));
        
        ui = UIManager.GetComponent<UI>();       
        cubeRenderer = GetComponent<Renderer>();

        var buildingCubeExpand = new BuildingCubeExpand(gameObject, Cube, BuildingAreas);

        redColor = new Color(0.3f, 0.1f, 0.1f, 0.6f);
        greenColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
        blueColor = new Color(0.1f, 0.1f, 0.3f, 0.4f);
    }

    private void Update()
    {
        if (ui.IsBuildModeActive)
        {
            isTransparent = false;
            cubeRenderer.material.color = blueColor;        

            if (IsBusy)
            {
                cubeRenderer.material.color = redColor;
                IsChosen = false;
            }
           
            if (IsChosen)       
                cubeRenderer.material.color = greenColor;           
        }
        else
        {
            if(!isTransparent)
            {
                cubeRenderer.material.color = new Color(0, 0, 0, 0);
                isTransparent = true;
            }
        }
    }
}
