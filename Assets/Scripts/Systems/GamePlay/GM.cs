
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

        [NaughtyAttributes.BoxGroup("Prefab")]
        public GameObject CellPrefab, CreepPrefab, TowerPrefab, RangePrefab, CreepSpawnPoint, LevelUpEffect;

        [NaughtyAttributes.BoxGroup("Parent")]
        public Transform CellParent, TowerParent, CreepParent;

        [NaughtyAttributes.BoxGroup("Data")]
        public PlayerData PlayerData;

        [NaughtyAttributes.BoxGroup("Data")]
        public TowerDataBase TowerDataBase;

        [NaughtyAttributes.BoxGroup("Data")]
        public CreepDataBase CreepDataBase;

        [NaughtyAttributes.BoxGroup("Data")]
        public WaveDataBase WaveDataBase;

        [NaughtyAttributes.BoxGroup("Data")]
        public int WaveAmount;

        public TowerPlaceSystem TowerPlaceSystem { get => towerPlaceSystem; set => towerPlaceSystem = value; }
        public GridSystem GridSystem { get => gridSystem; set => gridSystem = value; }
        public WaveSystem WaveSystem { get => waveSystem; set => waveSystem = value; }
        public PlayerInputSystem PlayerInputSystem { get => playerInputSystem; set => playerInputSystem = value; }
        public BaseUISystem BaseUISystem { get => baseUISystem; set => baseUISystem = value; }
        public TowerUISystem TowerUISystem { get => towerUISystem; set => towerUISystem = value; }
        public ResourceSystem ResourceSystem { get => resourceSystem; set => resourceSystem = value; }
        public ElementSystem ElementSystem { get => elementSystem; set => elementSystem = value; }
        public ElementUISystem ElementUISystem { get => elementUISystem; set => elementUISystem = value; }
        public TowerCreatingSystem TowerCreatingSystem { get => towerCreatingSystem; set => towerCreatingSystem = value; }
        public BuildUISystem BuildUISystem { get => buildUISystem; set => buildUISystem = value; }

        private TowerPlaceSystem towerPlaceSystem;
        private GridSystem gridSystem;
        private WaveSystem waveSystem;
        private PlayerInputSystem playerInputSystem;
        private BaseUISystem baseUISystem;
        private TowerUISystem towerUISystem;
        private ResourceSystem resourceSystem;
        private ElementSystem elementSystem;
        private ElementUISystem elementUISystem;
        private TowerCreatingSystem towerCreatingSystem;
        private BuildUISystem buildUISystem;

        public Canvas UICanvas;

        public static GM Instance;       
        public static State PlayerState;
        public static int[] ExpToLevelUp;
     
        public enum State
        {
            Idle,
            ChoosedCreep,
            ChoosedTower,
            PlacingTower,
            PreparePlacingTower
        }

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
                Instance = this;
            else
                Debug.Log("Warning: multiple " + this + " in scene!");

            GridSystem          = new GridSystem();
            TowerPlaceSystem    = new TowerPlaceSystem();
            TowerCreatingSystem = new TowerCreatingSystem();
            WaveSystem          = new WaveSystem();
            ElementSystem       = new ElementSystem();
            ResourceSystem      = new ResourceSystem();
            
            PlayerState = State.Idle;
        
            Application.targetFrameRate = 70;
            QualitySettings.vSyncCount = 0;
            Cursor.lockState = CursorLockMode.Confined;

            ExpToLevelUp = new int[25];

            ExpToLevelUp[0]     = 12;
            ExpToLevelUp[1]     = 24;
            ExpToLevelUp[2]     = 37;
            ExpToLevelUp[3]     = 51;
            ExpToLevelUp[4]     = 66;
            ExpToLevelUp[5]     = 82;
            ExpToLevelUp[6]     = 99;
            ExpToLevelUp[7]     = 117;
            ExpToLevelUp[8]     = 136;
            ExpToLevelUp[9]     = 156;
            ExpToLevelUp[10]    = 177;
            ExpToLevelUp[11]    = 199;
            ExpToLevelUp[12]    = 223;
            ExpToLevelUp[13]    = 248;
            ExpToLevelUp[14]    = 275;
            ExpToLevelUp[15]    = 303;
            ExpToLevelUp[16]    = 333;
            ExpToLevelUp[17]    = 365;
            ExpToLevelUp[18]    = 399;
            ExpToLevelUp[19]    = 435;
            ExpToLevelUp[20]    = 473;
            ExpToLevelUp[21]    = 513;
            ExpToLevelUp[22]    = 556;
            ExpToLevelUp[23]    = 601;
            ExpToLevelUp[24]    = 649;
        }

        private void Update()
        {
            TowerPlaceSystem.Update();
            GridSystem.Update();
            WaveSystem.Update();
        }
    }
}