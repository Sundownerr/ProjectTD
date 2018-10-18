
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{

    public class GM : ExtendedMonoBehaviour
    {
        public GameObject[] WaypointList, TowerCellAreaList;
        public List<GameObject> TowerList, CreepList, CellList;
        public List<TowerCells.Cell> CellStateList;
        public GameObject CellPrefab, CreepPrefab, TowerPrefab, RangePrefab, CreepSpawnPoint, LevelUpEffect;
        public Transform CellParent, BulletParent, TowerParent, CreepParent;
        public TowerPlaceSystem TowerPlaceSystem;
        public GridSystem GridSystem;
        public WaveSystem WaveSystem;
        public PlayerInputSystem PlayerInputSystem;
        public BaseUISystem BaseUISystem;
        public TowerUISystem TowerUISystem;
        public PlayerDataSystem PlayerDataSystem;
        public Data.PlayerData PlayerData;
        public Canvas UICanvas;

        public static GM Instance;
        public static int PLAYERSTATE, IDLE, PLACING_TOWER, CHOOSED_CREEP, CHOOSED_TOWER, PREPARE_PLACING_TOWER;
        public static int[] ExpToLevelUp;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("Warning: multiple " + this + " in scene!");
            }

            IDLE = 0;
            CHOOSED_CREEP = 1;
            CHOOSED_TOWER = 2;
            PLACING_TOWER = 3;
            PREPARE_PLACING_TOWER = 4;

            PLAYERSTATE = IDLE;

            Application.targetFrameRate = 70;

            QualitySettings.vSyncCount = 0;

            Cursor.lockState = CursorLockMode.Confined;

            ExpToLevelUp = new int[25];
            ExpToLevelUp[0] = 12;
            ExpToLevelUp[1] = 24;
            ExpToLevelUp[2] = 37;
            ExpToLevelUp[3] = 51;
            ExpToLevelUp[4] = 66;
            ExpToLevelUp[5] = 82;
            ExpToLevelUp[6] = 99;
            ExpToLevelUp[7] = 117;
            ExpToLevelUp[8] = 136;
            ExpToLevelUp[9] = 156;
            ExpToLevelUp[10] = 177;
            ExpToLevelUp[11] = 199;
            ExpToLevelUp[12] = 223;
            ExpToLevelUp[13] = 248;
            ExpToLevelUp[14] = 275;
            ExpToLevelUp[15] = 303;
            ExpToLevelUp[16] = 333;
            ExpToLevelUp[17] = 365;
            ExpToLevelUp[18] = 399;
            ExpToLevelUp[19] = 435;
            ExpToLevelUp[20] = 473;
            ExpToLevelUp[21] = 513;
            ExpToLevelUp[22] = 556;
            ExpToLevelUp[23] = 601;
            ExpToLevelUp[24] = 649;
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

        public static float GetPercentOfValue(float desiredPercent, float value)
        {
            return value / 100 * desiredPercent;
        }

        public static string KiloFormat(float num)
        {
            if (num >= 1000000000)
                return (num / 1000000000).ToString("#.0" + "B");

            if (num >= 1000000)
                return (num / 1000000).ToString("#" + "M");

            if (num >= 100000)
                return (num / 1000).ToString("#.0" + "K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#" + "K");

            if (num >= 1000)
                return (num / 1000).ToString("0.#" + "K");

            return num.ToString("0.#");
        }
    }
}