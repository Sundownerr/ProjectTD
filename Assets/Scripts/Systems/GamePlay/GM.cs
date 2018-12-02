
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Data;
using Game.Tower.Data;
using System.Threading;
using Game.Cells;
using Game.Creep;
using Game.Tower;

namespace Game.Systems
{
    public enum State
    {
        Idle,
        ChoosedCreep,
        ChoosedTower,
        PlacingTower,
        PreparePlacingTower
    }

    [Serializable]
    public class GM : ExtendedMonoBehaviour
    {
        [NaughtyAttributes.BoxGroup("List")]
        public GameObject[] WaypointList, CellAreaList, ElementPlaceEffectList;  

        [NaughtyAttributes.BoxGroup("List")]
        public List<TowerData> AvailableTowerList;     

        [NaughtyAttributes.BoxGroup("Prefab")]
        public GameObject CellPrefab, CreepPrefab, TowerPrefab, RangePrefab, CreepSpawnPoint, LevelUpEffect;

        [NaughtyAttributes.BoxGroup("Parent")]
        public Transform CellParent, TowerParent, CreepParent;

        [NaughtyAttributes.BoxGroup("Data")]
        public int WaveAmount;
        public TowerDataBase TowerDataBase;             
        public CreepDataBase CreepDataBase;            
        public WaveDataBase WaveDataBase;     

        public List<TowerSystem> PlacedTowerList = new List<TowerSystem>();

        public TowerPlaceSystem TowerPlaceSystem        { get => towerPlaceSystem; set => towerPlaceSystem = value; }
        public GridSystem GridSystem                    { get => gridSystem; set => gridSystem = value; }
        public WaveSystem WaveSystem                    { get => waveSystem; set => waveSystem = value; }
        public PlayerInputSystem PlayerInputSystem      { get => playerInputSystem; set => playerInputSystem = value; }
        public BaseUISystem BaseUISystem                { get => baseUISystem; set => baseUISystem = value; }
        public TowerUISystem TowerUISystem              { get => towerUISystem; set => towerUISystem = value; }
        public ResourceSystem ResourceSystem            { get => resourceSystem; set => resourceSystem = value; }     
        public ElementUISystem ElementUISystem          { get => elementUISystem; set => elementUISystem = value; }
        public TowerCreatingSystem TowerCreatingSystem  { get => towerCreatingSystem; set => towerCreatingSystem = value; }
        public BuildUISystem BuildUISystem              { get => buildUISystem; set => buildUISystem = value; }
        public List<CreepSystem> CreepList        { get => creepList; set => creepList = value; }
        public WaveUISystem WaveUISystem                { get => waveUISystem; set => waveUISystem = value; }
        public TowerControlSystem TowerControlSystem    { get => towerControlSystem; set => towerControlSystem = value; }
        public CreepControlSystem CreepControlSystem    { get => creepControlSystem; set => creepControlSystem = value; }
        public PlayerData PlayerData                    { get => playerData; set => playerData = value; }
        public ElementSystem ElementSystem              { get => elementSystem; set => elementSystem = value; }
                   
        private PlayerData playerData;   
        private TowerPlaceSystem towerPlaceSystem;
        private GridSystem gridSystem;
        private WaveSystem waveSystem;
        private PlayerInputSystem playerInputSystem;
        private BaseUISystem baseUISystem;
        private WaveUISystem waveUISystem;
        private TowerUISystem towerUISystem;
        private ResourceSystem resourceSystem;
        private ElementSystem elementSystem;
        private TowerControlSystem towerControlSystem;
        private CreepControlSystem creepControlSystem;
        private ElementUISystem elementUISystem;
        private TowerCreatingSystem towerCreatingSystem;
        private BuildUISystem buildUISystem;
        private List<CreepSystem> creepList = new List<CreepSystem>();

        public Canvas UICanvas;

        public static GM I;       
        public static State PlayerState;
        public static int[] ExpToLevelUp;
           
        protected override void Awake()
        {
            base.Awake();

            if (I == null)
                I = this;
            else
                Debug.Log("Warning: multiple " + this + " in scene!");

            gridSystem          = new GridSystem();
            resourceSystem      = new ResourceSystem();
            elementSystem       = new ElementSystem();
            towerPlaceSystem    = new TowerPlaceSystem();
            towerCreatingSystem = new TowerCreatingSystem();
            waveSystem          = new WaveSystem();     
            towerControlSystem  = new TowerControlSystem();    
            creepControlSystem  = new CreepControlSystem();

            PlayerData      = DataLoadingSystem.Load<PlayerData>() as PlayerData;
           
            
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

        private void Start()
        {
            waveSystem.SetSystem();
            towerControlSystem.SetSystem();
            waveUISystem.SetSystem();     
            resourceSystem.SetSystem();          
            towerCreatingSystem.SetSystem();    
            CreepControlSystem.SetSystem();              
        }

        private void Update()
        {
            waveSystem.UpdateSystem();
            towerControlSystem.UpdateSystem();
            towerPlaceSystem.UpdateSystem();
            gridSystem.UpdateSystem();           
            creepControlSystem.UpdateSystem();         
        }      
    }

    public static class StaticRandom
    {
        private static int seed;

        private static ThreadLocal<System.Random> threadLocal = new ThreadLocal<System.Random>
                        (() => new System.Random(Interlocked.Increment(ref seed)));

        static StaticRandom() => seed = Environment.TickCount;
    
        public static System.Random Instance { get { return threadLocal.Value; } }
    }

    public static class QoL
    {
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