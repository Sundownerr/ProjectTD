using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{

    public bool IsCreepInRange;
    public List<GameObject> CreepInRangeList;

    private void Start()
    {
        CreepInRangeList = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CreepInRangeList.Add(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        IsCreepInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        CreepInRangeList.RemoveAt(0);
        Debug.Log(CreepInRangeList.Count);
    }

}
