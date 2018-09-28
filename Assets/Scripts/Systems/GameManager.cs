
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{

    public class GameManager : MonoBehaviour
    {
        public GameObject[]  WaypointList, TowerCellAreaList;
        public List<GameObject> TowerList, CreepList, TowerCellList;
        public GameObject TowerCellPrefab, CreepPrefab, TowerPrefab, CreepSpawnPoint;
        public Transform TowerCellParent, BulletParent, TowerParent, CreepParent;
        public TowerPlaceSystem TowerPlaceSystem;
        public GridSystem GridSystem;
        public WaveSystem WaveSystem;
        public PlayerSystem PlayerSystem;
        public BaseUISystem UISystem;
        public TowerUISystem TowerUISystem;
        public Canvas UICanvas;



        public static GameManager Instance;

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