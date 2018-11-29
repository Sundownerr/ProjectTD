using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using UnityEngine;
using System;
using Game.Creep.Data;
using U = UnityEngine.Object;

namespace Game.Systems
{
    public class CreepEventArgs
    {
        public CreepSystem Creep { get => creep; set => creep = value; }
        private CreepSystem creep;

        public CreepEventArgs(CreepSystem creep)
        {
            this.creep = creep;
        }    
    }
    public class WaveSystem 
    {
        public int WaveNumber { get => waveNumber; set => waveNumber = value > 0 ? value : 1; }
        public List<CreepData> CurrentWaveCreepList { get => currentWaveCreepList; set => currentWaveCreepList = value; }
        public event EventHandler WaveChanged = delegate{};
        public event EventHandler<CreepEventArgs> CreepSpawned = delegate{};
        public event EventHandler AllWaveCreepsKilled = delegate{};

        private int waveNumber;
        private List<List<CreepSystem>> creepWaveList;
        private List<List<CreepData>> waveList;
        private List<CreepData> currentWaveCreepList;

        public WaveSystem()
        {
            GM.I.WaveSystem = this;

            currentWaveCreepList    = new List<CreepData>();
            creepWaveList           = new List<List<CreepSystem>>();           
            waveList                = new List<List<CreepData>>();

            waveList = CreateWaveList(GM.I.WaveAmount);
            waveNumber = 1;
            currentWaveCreepList = waveList[0];         
        }

        public void SetSystem()
        {
            GM.I.BaseUISystem.WaveStarted += OnWaveStarted;
        }

        private List<List<CreepData>> CreateWaveList(int waveAmount)
        {
            var armorRandomList = new List<int>();
            var waveRandomList  = new List<int>();         
            var armorTypeList   = Enum.GetValues(typeof(Armor.ArmorType));
            var raceTypeList    = Enum.GetValues(typeof(RaceType));     
            var tempWaveList    = new List<List<CreepData>>();      
            var waveList        = GM.I.WaveDataBase.WaveList;      

            for (int i = 0; i < waveAmount; i++)
            {
                waveRandomList.Add(StaticRandom.Instance.Next(0, waveList.Count));
                armorRandomList.Add(StaticRandom.Instance.Next(0, armorTypeList.Length));      
            }

            for (int waveId = 0; waveId < waveAmount; waveId++)
            {             
                tempWaveList.Add(
                    WaveCreatingSystem.CreateWave(
                        RaceType.Humanoid,
                        waveList[waveRandomList[waveId]]));               
            }   
            return tempWaveList;
        }

        private CreepData CalculateStats(CreepData stats, Armor.ArmorType armor, int waveCount)
        {
            var tempStats = U.Instantiate(stats);
           
            tempStats.ArmorType         = armor;
            tempStats.ArmorValue        = waveCount;
            tempStats.DefaultMoveSpeed  = 120 + waveCount * 5;
            tempStats.Gold              = 1 + waveCount;        // waveCount / 7;
            tempStats.Health            = waveCount * 10;
            tempStats.MoveSpeed         = tempStats.DefaultMoveSpeed;

            tempStats.IsInstanced = true;
            return tempStats;
        }

        public void UpdateSystem()
        {
            AddMagicCrystalAfterWaveEnd();

            void AddMagicCrystalAfterWaveEnd()
            {
                for (int waveId = 0; waveId < creepWaveList.Count; waveId++)                 
                    if (creepWaveList[waveId].Count > 0)
                    {
                        for (int creepId = 0; creepId < creepWaveList[waveId].Count; creepId++)
                            if (creepWaveList[waveId][creepId] == null)
                                creepWaveList[waveId].RemoveAt(creepId);                    
                    }
                    else
                    {
                        AllWaveCreepsKilled?.Invoke(this, new EventArgs());
                        creepWaveList.RemoveAt(waveId);
                    }                       
            }
        }

        public void OnWaveStarted(object sender, EventArgs e)
        {          
            creepWaveList.Add(new List<CreepSystem>());
            GM.I.StartCoroutine(SpawnCreepWave(0.2f));

            IEnumerator SpawnCreepWave(float delay)
            {
                var spawned = 0;
                
                while (spawned < currentWaveCreepList.Count)
                {
                    SpawnCreep();
                    spawned++;
                    yield return new WaitForSeconds(delay);
                }
                
                if (waveNumber <= GM.I.WaveAmount)
                {
                    currentWaveCreepList = waveList[waveNumber];                 
                    GM.I.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                    waveNumber++;
                }

                void SpawnCreep()
                {
                    var creep = U.Instantiate(
                        currentWaveCreepList[spawned].Prefab, 
                        GM.I.CreepSpawnPoint.transform.position,
                        Quaternion.identity, 
                        GM.I.CreepParent);

                    var creepSystem = new CreepSystem(creep);                          
                    
                    creepSystem.Stats = CalculateStats(
                        currentWaveCreepList[spawned], 
                        currentWaveCreepList[spawned].ArmorType, 
                        waveNumber); 
                    creepSystem.Stats.SetData(creepSystem);
                    creepSystem.SetSystem();
             
                    creepWaveList[creepWaveList.Count - 1].Add(creepSystem);    
                        
                    CreepSpawned?.Invoke(this, new CreepEventArgs(creepSystem));
                    creepSystem.HealthSystem.CreepDied += GM.I.ResourceSystem.OnCreepDied;             
                }
            }
        }
    }
}
