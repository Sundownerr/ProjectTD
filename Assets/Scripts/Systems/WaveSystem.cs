using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class WaveSystem 
    {
        public int WaveCount;

        private StateMachine state;
        private List<List<GameObject>> creepWaveList;

        public WaveSystem()
        {
            creepWaveList = new List<List<GameObject>>();

            state = new StateMachine();
            state.ChangeState(new GetInputState(this));

            GM.Instance.WaveSystem = this;
        }

        public void Update()
        {
            state.Update();

            AddMagicCrystalAfterWaveEnd();
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
                var creep = Object.Instantiate(GM.Instance.CreepPrefab);

                creepWaveList[creepWaveList.Count - 1].Add(creep);

                spawnedCreepCount++;
                yield return new WaitForSeconds(spawnDelay);
            }

            state.ChangeState(new GetInputState(this));
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

                    GM.Instance.StartCoroutine(o.SpawnCreeps(21, 0.5f));
                    o.state.ChangeState(new SpawnCreepsState(o));
                }
            }

            public void Exit() => GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(false);                   
        }
    }
}
