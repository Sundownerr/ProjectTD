using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject TowerPlacer, UIManager;
    public bool IsTowerBuilded;

    private Transform towerRangeTransform, towerTransform;
    private List<Renderer> towerRendererList;
    private TowerPlacer towerPlacerData;       
    private RangeCollider rangeCollider;
    private UI ui;

    private void StartTowerBuild(Vector3 towerPos)
    {
        towerTransform.position = towerPos;

        for (int i = 0; i < towerRendererList.Count; i++)
        {
            towerRendererList[i].material.color = towerPlacerData.GhostedTowerColor;
        }
    }

    private bool EndTowerBuild()
    {
        for (int i = 0; i < towerRendererList.Count; i++)
        {
            towerRendererList[i].material.color = Color.blue;
        }
       
        towerRangeTransform.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

        return true;
    }

    private void Start ()
    {
        TowerPlacer = GameObject.Find("TowerManager");
        UIManager = GameObject.Find("UIManager");
        ui = UIManager.GetComponent<UI>();       
        towerPlacerData = TowerPlacer.GetComponent<TowerPlacer>();
        towerTransform = transform;
        towerRendererList = new List<Renderer>();

        for (int i = 0; i < GetComponentsInChildren<Renderer>().Length; i++)
        {
            towerRendererList.Add(GetComponentsInChildren<Renderer>()[i]);
        }      

        towerRangeTransform = transform.GetChild(1);

        rangeCollider = towerRangeTransform.gameObject.GetComponent<RangeCollider>();

        var randomNumber = Random.Range(5, 30);

        towerRangeTransform.localScale = new Vector3(randomNumber, 0.0001f, randomNumber);

	}

    private void Update()
    {
        if (!IsTowerBuilded && ui.IsBuildModeActive)
        {
            StartTowerBuild(towerPlacerData.GhostedTowerPosition);
        }
        else
        {
            IsTowerBuilded = EndTowerBuild();
        }
        
        if (IsTowerBuilded)
        {
            if (rangeCollider.IsCreepInRange)
            {
                var offset = rangeCollider.CreepInRangeList[0].transform.position - towerTransform.position;
                offset.y = 0;

                var towerRotation = Quaternion.LookRotation(offset);

                towerTransform.rotation = Quaternion.Lerp(towerTransform.rotation, towerRotation, Time.deltaTime * 3.1f);
            }
            else
            {
                if (towerTransform.rotation != Quaternion.Euler(0, 0, 0))
                {
                    towerTransform.rotation = Quaternion.Lerp(towerTransform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 1f);
                }
            }
        }
    }    
}
