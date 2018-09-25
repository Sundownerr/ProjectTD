using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridSystem : MonoBehaviour
{ 
    public bool IsGridBuilded;

    private IEnumerator BuildTimer()
    {
        CreateGrid(GameManager.Instance.TowerCellAreaList.Count);

        yield return new WaitForSeconds(0.5f); 
        
        IsGridBuilded = true; 
    }

    private void CreateGrid(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var ray = new Ray(GameManager.Instance.TowerCellAreaList[i].transform.position, Vector3.up);
            var layerMask = 1 << 9;
            layerMask = ~layerMask;

            if (!Physics.Raycast(ray, 100, layerMask))
            {              
                var spawnPos = GameManager.Instance.TowerCellAreaList[i].transform.position + new Vector3(0, GameManager.Instance.TowerCellAreaList[i].transform.localScale.y / 1.9f, 0);
                var towerCell = Instantiate(GameManager.Instance.TowerCellPrefab, spawnPos, Quaternion.Euler(0, 0, 0)); 
                towerCell.name = GameManager.Instance.TowerCellAreaList[i].name + " Cell";           
            }
        }
    }

    private void Start()
    {                      
        StartCoroutine(BuildTimer());
    }
}
