﻿using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using UnityEngine;
using System;
using Game.Creep.Data;
using U = UnityEngine.Object;
using Game.Data;

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
        public List<CreepData> CurrentWaveCreeps { get => currentWaveCreeps; set => currentWaveCreeps = value; }
        public event EventHandler WaveChanged = delegate{};
        public event EventHandler<CreepEventArgs> CreepSpawned = delegate{};
        public event EventHandler AllWaveCreepsKilled = delegate{};

        private int waveNumber;
        private List<List<CreepSystem>> creepWaves;
        private List<List<CreepData>> waves;
        private List<CreepData> currentWaveCreeps;

        public WaveSystem()
        {
            GM.I.WaveSystem = this;

            currentWaveCreeps = new List<CreepData>();
            creepWaves        = new List<List<CreepSystem>>();           
            waves             = new List<List<CreepData>>();  
        }

        public void SetSystem()
        {
            GM.I.BaseUISystem.WaveStarted += OnWaveStarted;

            waves = GenerateWaves(GM.I.WaveAmount);
            waveNumber = 1;
            currentWaveCreeps = waves[0];      

            #region  Helper functions

            List<List<CreepData>> GenerateWaves(int waveAmount)
            {             
                var randomWaveIds  = new List<int>();                    
                var raceTypes    = Enum.GetValues(typeof(RaceType));     
                var tempWaves    = new List<List<CreepData>>();      
                var waves        = GM.I.WaveDataBase.Waves;      

                for (int i = 0; i < waveAmount; i++)               
                    randomWaveIds.Add(StaticRandom.Instance.Next(0, waves.Count));

                for (int i = 0; i < waveAmount; i++)                         
                    tempWaves.Add(WaveCreatingSystem.CreateWave(waves[randomWaveIds[i]], i));               
                  
                return tempWaves;
            }   

            #endregion
        }

        public void UpdateSystem()
        {
            AddMagicCrystalAfterWaveEnd();

            #region  Helper functions

            void AddMagicCrystalAfterWaveEnd()
            {
                for (int waveId = 0; waveId < creepWaves.Count; waveId++)                 
                    if (creepWaves[waveId].Count > 0)
                    {
                        for (int creepId = 0; creepId < creepWaves[waveId].Count; creepId++)
                            if (creepWaves[waveId][creepId] == null)
                                creepWaves[waveId].RemoveAt(creepId);                    
                    }
                    else
                    {
                        AllWaveCreepsKilled?.Invoke(this, new EventArgs());
                        creepWaves.RemoveAt(waveId);
                    }                       
            }

            #endregion
        }

        public void OnWaveStarted(object sender, EventArgs e)
        {          
            creepWaves.Add(new List<CreepSystem>());
            GM.I.StartCoroutine(SpawnCreepWave(0.2f));

            #region  Helper functions

            IEnumerator SpawnCreepWave(float delay)
            {
                var spawned = 0;
                
                while (spawned < currentWaveCreeps.Count)
                {
                    SpawnCreep();
                    spawned++;
                    yield return new WaitForSeconds(delay);
                }
                
                if (waveNumber <= GM.I.WaveAmount)
                {
                    currentWaveCreeps = waves[waveNumber];                 
                    GM.I.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                    waveNumber++;
                    WaveChanged?.Invoke(this, new EventArgs());
                }

                #region  Helper functions

                void SpawnCreep()
                {
                    var creep = U.Instantiate(
                        currentWaveCreeps[spawned].Prefab, 
                        GM.I.CreepSpawnPoint.transform.position,
                        Quaternion.identity, 
                        GM.I.CreepParent);

                    var creepSystem = new CreepSystem(creep);                          
                    
                    creepSystem.Stats = currentWaveCreeps[spawned];
                    creepSystem.Stats.SetData(creepSystem);
                    creepSystem.SetSystem();
             
                    creepWaves[creepWaves.Count - 1].Add(creepSystem);    
                        
                    CreepSpawned?.Invoke(this, new CreepEventArgs(creepSystem));
                    creepSystem.HealthSystem.CreepDied += GM.I.ResourceSystem.OnCreepDied;              
                }
                #endregion
            }
            #endregion
        }
    }
}
