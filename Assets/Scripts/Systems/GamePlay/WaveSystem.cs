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
        public List<CreepData> CurrentWaveCreepList { get => currentWaveCreepList; set => currentWaveCreepList = value; }
        public event EventHandler WaveChanged = delegate{};

        private int waveNumber;
        private StateMachine state;
        private List<List<CreepSystem>> creepWaveList;
        private List<List<CreepData>> waveList;
        private List<CreepData> currentWaveCreepList;

        public WaveSystem()
        {
            GM.I.WaveSystem = this;

            currentWaveCreepList    = new List<CreepData>();
            creepWaveList           = new List<List<CreepSystem>>();           
            waveList                = new List<List<CreepData>>();

            state = new StateMachine();
            state.ChangeState(new GenerateWavesState(this, GM.I.WaveAmount));    
        }

        public void Update()
        {
            state.Update();
            AddMagicCrystalAfterWaveEnd();      
            HandleCreeps();                
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
            tempStats.Gold              = waveCount / 7;
            tempStats.Health            = waveCount * 10;
            tempStats.MoveSpeed         = tempStats.DefaultMoveSpeed;

            tempStats.IsInstanced = true;
            return tempStats;
        }

        private void HandleCreeps()
        {
            var creepList = GM.I.CreepSystemList;
            for (int i = 0; i < creepList.Count; i++)            
                CreepControlSystem.MoveToNextWaypoint(creepList[i]);      
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
                    GM.I.ResourceSystem.AddMagicCrystal(5);
                    creepWaveList.RemoveAt(waveId);
                }                       
        }

        protected class GetInputState : IState
        {
            private readonly WaveSystem o;

            public GetInputState(WaveSystem o) => this.o = o; 

            public void Enter() {o.WaveChanged?.Invoke(o, new EventArgs());}

            public void Execute()
            {
                if (GM.I.BaseUISystem.IsWaveStarted)                                    
                    o.state.ChangeState(new SpawnCreepsState(o));   
            }

            public void Exit() => GM.I.BaseUISystem.StartWaveButton.gameObject.SetActive(false);                   
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
                o.waveNumber = 1;
                o.currentWaveCreepList = o.waveList[0];       
                o.state.ChangeState(new GetInputState(o));
                GM.I.WaveUISystem.UpdateWaveUI(this, new EventArgs());
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
                o.creepWaveList.Add(new List<CreepSystem>());
                GM.I.StartCoroutine(SpawnCreepWave(0.2f));

                IEnumerator SpawnCreepWave(float delay)
                {
                    var spawned = 0;
                    
                    while (spawned < o.currentWaveCreepList.Count)
                    {
                        SpawnCreep();
                        spawned++;
                        yield return new WaitForSeconds(delay);
                    }
                    
                    o.state.ChangeState(new GetInputState(o));

                    void SpawnCreep()
                    {
                        var creep = U.Instantiate(
                            o.currentWaveCreepList[spawned].Prefab, 
                            GM.I.CreepSpawnPoint.transform.position,
                            Quaternion.identity, 
                            GM.I.CreepParent);

                        var creepSystem = creep.GetComponent<CreepSystem>();                          

                        creepSystem.Stats = o.CalculateStats(
                            o.currentWaveCreepList[spawned], 
                            o.currentWaveCreepList[spawned].ArmorType, 
                            o.waveNumber); 

                        creepSystem.HealthSystem = new HealthSystem(creepSystem);
                        creepSystem.EffectSystem = new EffectSystem();             
                        creepSystem.IsVulnerable = true;  

                        GM.I.CreepList.Add(creep);
                        GM.I.CreepSystemList.Add(creepSystem);    
                        o.creepWaveList[o.creepWaveList.Count - 1].Add(creepSystem);                                             
                    }
                }
            }

            public void Execute() { }

            public void Exit()
            {
                if(o.waveNumber <= GM.I.WaveAmount)
                {
                    o.currentWaveCreepList = o.waveList[o.waveNumber];
                    GM.I.BaseUISystem.IsWaveStarted = false;
                    GM.I.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                    o.waveNumber++;
                }
            }
        }    
    }
}
