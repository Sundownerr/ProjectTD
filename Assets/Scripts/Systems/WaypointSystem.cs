
using System.Collections.Generic;
using UnityEngine;

public class WaypointSystem : MonoBehaviour
{
    public List<GameObject> Waypoints;

    private void Start()
    {
        GameManager.Instance.WaypointList = Waypoints;
    }
}
