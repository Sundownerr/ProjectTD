using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingCube : MonoBehaviour
{
    public GameObject BuildingCubee, UIManager, TowerPlacer;
    public bool isBusy, isChosen;

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

        var buildingCubeExpand = new BuildingCubeExpand(gameObject, BuildingCubee, BuildingAreas);

        redColor = new Color(0.3f, 0.1f, 0.1f, 0.6f);
        greenColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
        blueColor = new Color(0.1f, 0.1f, 0.3f, 0.4f);
    }

    private void Update()
    {
        if (ui.BuildModeActive)
        {
            isTransparent = false;
            cubeRenderer.material.color = blueColor;        

            if (isBusy)
            {
                cubeRenderer.material.color = redColor;
                isChosen = false;
            }
           
            if (isChosen)       
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
