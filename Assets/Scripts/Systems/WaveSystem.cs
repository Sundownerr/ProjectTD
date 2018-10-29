using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class WaveSystem : ExtendedMonoBehaviour
    {
        public int WaveCount;

        private StateMachine state;
        private List<List<GameObject>> waveCreepList;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
                CachedTransform = transform;

            waveCreepList = new List<List<GameObject>>();

            state = new StateMachine();
            state.ChangeState(new GetInputState(this));

            GM.Instance.WaveSystem = this;
        }

        private void Update()
        {
            state.Update();

            AddMagicCrystalAfterWaveEnd();
        }
        
        private void AddMagicCrystalAfterWaveEnd()
        {
            if (waveCreepList.Count > 0)
                for (int i = 0; i < waveCreepList.Count; i++)             
                    if (waveCreepList[i].Count > 0)
                    {
                        for (int j = 0; j < waveCreepList[i].Count; j++)
                            if (waveCreepList[i][j] == null)
                                waveCreepList[i].RemoveAt(j);
                    }
                    else
                    {
                        GM.Instance.ResourceSystem.AddMagicCrystal(5);
                        waveCreepList.RemoveAt(i);
                    }                       
        }

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            var spawnedCreepCount = 0;
            state.ChangeState(new SpawnCreepsState(this));

            waveCreepList.Add(new List<GameObject>());
    
            while (spawnedCreepCount < needToSpawnCount)
            {
                var creep = Instantiate(GM.Instance.CreepPrefab);

                waveCreepList[waveCreepList.Count - 1].Add(creep);

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
                    owner.StartCoroutine(owner.SpawnCreeps(21, 0.5f));                   
            }

            public void Exit()
            {
                GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(false);
            }
        }
    }
}
