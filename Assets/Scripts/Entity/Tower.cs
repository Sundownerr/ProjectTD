using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject TowerPlacer, UIManager;
    public bool builded;

    private List<Renderer> towerRenderers;
    private TowerPlacer.TowerPlacerData towerData;
    private UI ui;
    private Transform towerRange;

	void Start ()
    {
        TowerPlacer = GameObject.Find("TowerManager");
        UIManager = GameObject.Find("UIManager");
        ui = UIManager.GetComponent<UI>();       
        towerData = TowerPlacer.GetComponent<TowerPlacer.TowerPlacerData>();

        towerRenderers = new List<Renderer>();

        for (int i = 0; i < GetComponentsInChildren<Renderer>().Length; i++)       
            towerRenderers.Add(GetComponentsInChildren<Renderer>()[i]);

        towerRange = transform.GetChild(1);

       
        towerRange.GetComponent<Renderer>().material.color = new Color(0, 0, 0.5f, 0.5f);

        var randomNumber = Random.Range(5, 30);

        towerRange.localScale = new Vector3(randomNumber, 0.0001f, randomNumber);

	}

    void Update()
    {
        if (!builded && ui.BuildModeActive)
        {
            transform.position = towerData.TowerPosition;

            for (int i = 0; i < towerRenderers.Count; i++)
                towerRenderers[i].material.color = towerData.TowerColor;
        }
        else
        {
            for (int i = 0; i < towerRenderers.Count; i++)
                towerRenderers[i].material.color = Color.blue;
            
            builded = true;

            towerRange.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        }
    }
}
