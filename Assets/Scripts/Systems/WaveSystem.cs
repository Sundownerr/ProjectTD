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
            private readonly WaveSystem owner;

            public SpawnCreepsState(WaveSystem owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute() { }

            public void Exit()
            {
                GM.Instance.BaseUISystem.IsWaveStarted = false;
                GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                owner.WaveCount++;
            }
        }

        protected class GetInputState : IState
        {
            private readonly WaveSystem owner;

            public GetInputState(WaveSystem owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                if (GM.Instance.BaseUISystem.IsWaveStarted)
                {
                    owner.creepWaveList.Add(new List<GameObject>());

                    GM.Instance.StartCoroutine(owner.SpawnCreeps(21, 0.5f));
                    owner.state.ChangeState(new SpawnCreepsState(owner));
                }
            }

            public void Exit()
            {
                GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(false);
            }
        }
    }
}
