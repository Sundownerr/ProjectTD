
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class WaypointSystem : MonoBehaviour
    {
        public List<GameObject> Waypoints;

        private void Start()
        {
            GameManager.Instance.WaypointList = Waypoints;
        }
    }
}
