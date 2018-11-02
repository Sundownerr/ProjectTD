
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Data;
using Game.Tower.Data;

namespace Game.System
{
    [Serializable]
    public class GM : ExtendedMonoBehaviour
    {
        [NaughtyAttributes.BoxGroup("List")]
        public GameObject[] WaypointList, CellAreaList, ElementPlaceEffectList;

        [NaughtyAttributes.BoxGroup("List")]
        public List<GameObject> CreepList, CellList, PlacedTowerList;

        [NaughtyAttributes.BoxGroup("List")]
        public List<TowerData> AvailableTowerList;

        [NaughtyAttributes.BoxGroup("List")]
        public List<Cells.Cell> CellStateList;

        [NaughtyAttributes.BoxGroup("List")]
        public string[] ElementNameList;

        [NaughtyAttributes.BoxGroup("Prefab")]
        public GameObject CellPrefab, CreepPrefab, TowerPrefab, RangePrefab, CreepSpawnPoint, LevelUpEffect;

        [NaughtyAttributes.BoxGroup("Parent")]
        public Transform CellParent, TowerParent, CreepParent;

        [HideInInspector]
        public TowerPlaceSystem TowerPlaceSystem;

        [HideInInspector]
        public GridSystem GridSystem;

        [HideInInspector]
        public WaveSystem WaveSystem;

        [NaughtyAttributes.BoxGroup("Data")]
        public PlayerData PlayerData;

        [NaughtyAttributes.BoxGroup("Data")]
        public AllTowerData AllTowerData;

        [HideInInspector]
        public PlayerInputSystem PlayerInputSystem;

        [HideInInspector]
        public BaseUISystem BaseUISystem;

        [HideInInspector]
        public TowerUISystem TowerUISystem;

        [HideInInspector]
        public ResourceSystem ResourceSystem;

        [HideInInspector]
        public ElementSystem ElementSystem;

        [HideInInspector]
        public ElementUISystem ElementUISystem;

        [HideInInspector]
        public TowerCreatingSystem TowerCreatingSystem;

        [HideInInspector]
        public BuildUISystem BuildUISystem;

        public Canvas UICanvas;

        public static GM Instance;
        public static int PLAYERSTATE, IDLE, PLACING_TOWER, CHOOSED_CREEP, CHOOSED_TOWER, PREPARE_PLACING_TOWER;
        public static int[] ExpToLevelUp;

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
                Instance = this;
            else
                Debug.Log("Warning: multiple " + this + " in scene!");

            GridSystem = new GridSystem();
            TowerPlaceSystem = new TowerPlaceSystem();
            TowerCreatingSystem = new TowerCreatingSystem();
            WaveSystem = new WaveSystem();
            ElementSystem = new ElementSystem();
            ResourceSystem = new ResourceSystem();

            IDLE = 0;
            CHOOSED_CREEP = 1;
            CHOOSED_TOWER = 2;
            PLACING_TOWER = 3;
            PREPARE_PLACING_TOWER = 4;

            PLAYERSTATE = IDLE;

            Application.targetFrameRate = 70;
            QualitySettings.vSyncCount = 0;
            Cursor.lockState = CursorLockMode.Confined;

            ElementNameList = new string[]
              {
                "Astral",
                "Darkness",
                "Ice",
                "Iron",
                "Storm",
                "Nature",
                "Fire"
              };

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

        private void Update()
        {
            TowerPlaceSystem.Update();
            GridSystem.Update();
            WaveSystem.Update();
        }
    }
}