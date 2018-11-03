using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using UnityEngine;
using System;
using Game.Creep.Data;

namespace Game.System
{
    public class WaveSystem 
    {
        public int WaveCount { get => waveCount; set => waveCount = value; }

        private int waveCount;
        private StateMachine state;
        private List<List<GameObject>> creepWaveList;
        private WaveCreatingSystem waveCreatingSystem;
        private List<CreepData> currentWaveCreepList;
        private List<List<CreepData>> waveList;

        public WaveSystem()
        {
            creepWaveList = new List<List<GameObject>>();
            currentWaveCreepList = new List<CreepData>();
            waveList = new List<List<CreepData>>();

            state = new StateMachine();
            state.ChangeState(new GenerateWavesState(this, GM.Instance.WaveAmount));

            GM.Instance.WaveSystem = this;
        }

        public void Update()
        {
            state.Update();

            AddMagicCrystalAfterWaveEnd();
        }

        private List<List<CreepData>> CreateWaveList(int waveAmount)
        {
            var raceTypeList    = Enum.GetValues(typeof(RaceType)); 
            var armorTypeList   = Enum.GetValues(typeof(Armor.ArmorType));
            var waveList = GM.Instance.WaveDataBase.WaveList;
            var tempWaveList = new List<List<CreepData>>();

            for (int waveCount = 0; waveCount < waveAmount; waveCount++)
            {
                var race = RaceType.Humanoid;
                var armor = Armor.ArmorType.Plate;
                var wave = waveList[0];

                tempWaveList.Add(AdjustCreepStats(waveCreatingSystem.CreateWave(race, waveCount, wave), armor, waveCount));
            }   
            return tempWaveList;
        }

        private List<CreepData> AdjustCreepStats(List<CreepData> waveCreepList, Armor.ArmorType armorType, int waveCount)
        {
            var tempCreepList = waveCreepList;

            for (int i = 0; i < tempCreepList.Count; i++)
                tempCreepList[i] = CalculateStats(tempCreepList[i], armorType, waveCount);   

            return tempCreepList;                  
        }

        private CreepData CalculateStats(CreepData stats, Armor.ArmorType armor, int waveCount)
        {
            var tempStats = stats;

            tempStats.ArmorType         = Armor.ArmorType.Chainmail;
            tempStats.ArmorValue        += waveCount;
            tempStats.DefaultMoveSpeed  += waveCount + 3;
            tempStats.Gold              += waveCount / 7;
            tempStats.Health            += waveCount * 10;
            
            return tempStats;
        }
        
        private void AddMagicCrystalAfterWaveEnd()
        {
            if (creepWaveList.Count > 0)
                for (int waveId = 0; waveId < creepWaveList.Count; waveId++)   
                    
                    if (creepWaveList[waveId].Count > 0)
                    {
                        for (int creepId = 0; creepId < creepWaveList[waveId].Count; creepId++)
                            if (creepWaveList[waveId][creepId] == null)
                                creepWaveList[waveId].RemoveAt(creepId);
                    }
                    else
                    {
                        GM.Instance.ResourceSystem.AddMagicCrystal(5);
                        creepWaveList.RemoveAt(waveId);
                    }                       
        }

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            var spawnedCreepCount = 0;
              
            while (spawnedCreepCount < needToSpawnCount)
            {
                var creep = UnityEngine.Object.Instantiate(currentWaveCreepList[spawnedCreepCount].Prefab);
                creep.GetComponent<CreepSystem>().SetStats(currentWaveCreepList[spawnedCreepCount]);

                creepWaveList[creepWaveList.Count - 1].Add(creep);

                spawnedCreepCount++;
                yield return new WaitForSeconds(spawnDelay);
            }

            state.ChangeState(new GetInputState(this));
        }

        protected class GenerateWavesState : IState
        {
            private readonly WaveSystem o;
            private int waveAmount;

            public GenerateWavesState(WaveSystem o, int waveAmount) 
            {
                this.o = o;
                this.waveAmount = waveAmount;
            }

            public void Enter() 
            {
                o.waveList = o.CreateWaveList(waveAmount);
               // o.currentWaveCreepList = o.waveList[0];
                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class SpawnCreepsState : IState
        {
            private readonly WaveSystem o;

            public SpawnCreepsState(WaveSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.BaseUISystem.IsWaveStarted = false;
                GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                o.WaveCount++;
            }
        }

        protected class GetInputState : IState
        {
            private readonly WaveSystem o;

            public GetInputState(WaveSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute()
            {
                if (GM.Instance.BaseUISystem.IsWaveStarted)
                {
                    o.creepWaveList.Add(new List<GameObject>());

                    GM.Instance.StartCoroutine(o.SpawnCreeps(o.currentWaveCreepList.Count, 0.5f));
                    o.state.ChangeState(new SpawnCreepsState(o));
                }
            }

            public void Exit() => GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(false);                   
        }
    }
}
