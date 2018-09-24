using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UI UISystem;
    public GridSystem GridSystem;
    public TowerPlaceSystem TowerPlaceSystem;
    public WaveSystem WaveSystem;
    public List<GameObject> TowerCellList, TowerCellAreaList, TowerList, CreepList;
    public GameObject TowerCellPrefab, CreepPrefab, TowerPrefab;
    public bool GameCanStart;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Warning: multiple " + this + " in scene!");
        }
    }
}
