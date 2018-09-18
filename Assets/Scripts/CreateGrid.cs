using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CreateGrid : MonoBehaviour
{
    public GameObject BuildingCube;
    public bool isGridBuilded;

    private List<GameObject> BuildingAreas;

    private IEnumerator BuildTimer()
    {
        yield return new WaitForSeconds(0.5f);        
        isGridBuilded = true;
    }

    private void GridCreate(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var ray = new Ray(BuildingAreas[i].transform.position, Vector3.up);
            var layerMask = 1 << 9;
            layerMask = ~layerMask;

            if (!Physics.Raycast(ray, 100, layerMask))
            {              
                var spawnPos = BuildingAreas[i].transform.position + new Vector3(0, BuildingAreas[i].transform.localScale.y / 1.9f, 0);
                var cube = Instantiate(BuildingCube, spawnPos, Quaternion.Euler(0, 0, 0)); 
                cube.name = BuildingAreas[i].name + " Cube";           
            }
        }
    }

    private void Start()
    {
        BuildingAreas = new List<GameObject>(GameObject.FindGameObjectsWithTag("BuildingArea"));
        GridCreate(BuildingAreas.Count);
        StartCoroutine(BuildTimer());
    }
}
