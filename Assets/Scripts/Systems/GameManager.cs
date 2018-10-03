
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{

    public class GameManager : MonoBehaviour
    {
        public GameObject[]  WaypointList, TowerCellAreaList;
        public List<GameObject> TowerList, CreepList, CellList;
        public GameObject CellPrefab, CreepPrefab, TowerPrefab, RangePrefab, CreepSpawnPoint;
        public Transform CellParent, BulletParent, TowerParent, CreepParent;
        public TowerPlaceSystem TowerPlaceSystem;
        public GridSystem GridSystem;
        public WaveSystem WaveSystem;
        public PlayerSystem PlayerSystem;
        public BaseUISystem UISystem;
        public TowerUISystem TowerUISystem;
        public Canvas UICanvas;

        public static GameManager Instance;
        public static int PLAYERSTATE, PLAYERSTATE_IDLE, PLAYERSTATE_PLACINGTOWER, PLAYERSTATE_CHOOSEDCREEP, PLAYERSTATE_CHOOSEDTOWER;

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

            PLAYERSTATE_IDLE = 0;
            PLAYERSTATE_CHOOSEDCREEP = 1;
            PLAYERSTATE_CHOOSEDTOWER = 2;
            PLAYERSTATE_PLACINGTOWER = 3;

            PLAYERSTATE = PLAYERSTATE_IDLE;
        }

        public static float CalcDistance(Vector3 pos1, Vector3 pos2)
        {

            Vector3 heading;
            float distanceSquared;
            float distance;

            heading.x = pos1.x - pos2.x;
            heading.y = pos1.y - pos2.y;
            heading.z = pos1.z - pos2.z;

            distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
            distance = Mathf.Sqrt(distanceSquared);

            return distance;
        }
    }
}