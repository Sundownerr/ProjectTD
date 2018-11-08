using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using UnityEngine;
using System;
using Game.Creep.Data;
using U = UnityEngine.Object;

namespace Game.Systems
{
    public class WaveSystem 
    {
        public int WaveNumber { get => waveNumber; set => waveNumber = value > 0 ? value : 1; }

        private int waveNumber;
        private StateMachine state;
        private WaveCreatingSystem waveCreatingSystem;
        private List<List<GameObject>> creepWaveList;
        private List<List<CreepData>> waveList;
        private List<CreepData> currentWaveCreepList;

        public WaveSystem()
        {
            GM.Instance.WaveSystem = this;

            currentWaveCreepList    = new List<CreepData>();
            creepWaveList           = new List<List<GameObject>>();           
            waveList                = new List<List<CreepData>>();
            waveCreatingSystem      = new WaveCreatingSystem();

            state = new StateMachine();
            state.ChangeState(new GenerateWavesState(this, GM.Instance.WaveAmount));    
        }

        public void Update()
        {
            state.Update();

            AddMagicCrystalAfterWaveEnd();                      
        }

        private List<List<CreepData>> CreateWaveList(int waveAmount)
        {
            var armorRandomList = new List<int>();
            var waveRandomList  = new List<int>();         
            var armorTypeList   = Enum.GetValues(typeof(Armor.ArmorType));
            var raceTypeList    = Enum.GetValues(typeof(RaceType));     
            var tempWaveList    = new List<List<CreepData>>();      
            var waveList        = GM.Instance.WaveDataBase.WaveList;      

            for (int i = 0; i < waveAmount; i++)
            {
                waveRandomList.Add(StaticRandom.Instance.Next(0, waveList.Count));
                armorRandomList.Add(StaticRandom.Instance.Next(0, armorTypeList.Length));        
            }

            for (int waveId = 0; waveId < waveAmount; waveId++)
            {
                var race    = RaceType.Humanoid;
                var armor   = (Armor.ArmorType)armorTypeList.GetValue(armorRandomList[waveId]);
                var wave    = GM.Instance.WaveDataBase.WaveList[waveRandomList[waveId]];
                
                tempWaveList.Add(waveCreatingSystem.CreateWave(race, waveId + 1, wave));
            }   
            return tempWaveList;
        }

        private CreepData CalculateStats(CreepData stats, Armor.ArmorType armor, int waveCount)
        {
            var tempStats = U.Instantiate(stats);
           
            tempStats.ArmorType         = armor;
            tempStats.ArmorValue        = waveCount;
            tempStats.DefaultMoveSpeed  = 120 + waveCount * 5;
            tempStats.Gold              = waveCount / 7;
            tempStats.Health            = waveCount * 10;
            tempStats.MoveSpeed         = tempStats.DefaultMoveSpeed;

            tempStats.IsInstanced = true;
            return tempStats;
        }
        
        private void AddMagicCrystalAfterWaveEnd()
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
                    GM.Instance.ResourceSystem.AddMagicCrystal(5);
                    creepWaveList.RemoveAt(waveId);
                }                       
        }

        private void SpawnCreep(CreepData creepStats)
        {
            var creep = U.Instantiate(creepStats.Prefab, GM.Instance.CreepParent);  
            creep.GetComponent<CreepSystem>().Stats = CalculateStats(creepStats, creepStats.ArmorType, waveNumber);

            creepWaveList[creepWaveList.Count - 1].Add(creep);
        }

        private IEnumerator SpawnCreepWave(int needToSpawn, float delay)
        {
            var spawned = 0;
              
            while (spawned < needToSpawn)
            {
                SpawnCreep(currentWaveCreepList[spawned]);
                spawned++;
                yield return new WaitForSeconds(delay);
            }

            state.ChangeState(new GetInputState(this));
        }

        protected class GetInputState : IState
        {
            private readonly WaveSystem o;

            public GetInputState(WaveSystem o) => this.o = o; 

            public void Enter() { }

            public void Execute()
            {
                if (GM.Instance.BaseUISystem.IsWaveStarted)                                    
                    o.state.ChangeState(new SpawnCreepsState(o));              
            }

            public void Exit() => GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(false);                   
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
                o.WaveNumber = 1;
                o.currentWaveCreepList = o.waveList[0];
                o.state.ChangeState(new GetInputState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class SpawnCreepsState : IState
        {
            private readonly WaveSystem o;

            public SpawnCreepsState(WaveSystem o) => this.o = o; 

            public void Enter() 
            {
                o.creepWaveList.Add(new List<GameObject>());
                GM.Instance.StartCoroutine(o.SpawnCreepWave(o.currentWaveCreepList.Count, 0.2f));
            }

            public void Execute() { }

            public void Exit()
            {
                if(o.WaveNumber <= GM.Instance.WaveAmount)
                {
                    o.currentWaveCreepList = o.waveList[o.WaveNumber];
                    GM.Instance.BaseUISystem.IsWaveStarted = false;
                    GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                    o.WaveNumber++;
                }
            }
        }    
    }
}
