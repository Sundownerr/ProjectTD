using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{
    public class WaveSystem : ExtendedMonoBehaviour
    {
        private bool isCreepSpawning, isCreepsSpawned;
        private StateMachine state;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            state = new StateMachine();
            state.ChangeState(new GetInputState(this));

            GM.Instance.WaveSystem = this;
        }

        private void Update()
        {
            state.Update();
        }

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            state.ChangeState(new SpawnCreepsState(this));

            var spawnedCreepCount = 0;         

            while (spawnedCreepCount < needToSpawnCount)
            {
                Instantiate(GM.Instance.CreepPrefab);

                spawnedCreepCount++;

                yield return new WaitForSeconds(spawnDelay);
            }

            state.ChangeState(new GetInputState(this));
        }

        protected class SpawnCreepsState : IState
        {
            private WaveSystem owner;

            public SpawnCreepsState(WaveSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {

            }

            public void Execute()
            {

            }

            public void Exit()
            {
                GM.Instance.BaseUISystem.IsWaveStarted = false;
                GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
            }
        }

        protected class GetInputState : IState
        {
            private WaveSystem owner;

            public GetInputState(WaveSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {              
            }
            
            public void Execute()
            {
                if (GM.Instance.BaseUISystem.IsWaveStarted)
                {
                    owner.StartCoroutine(owner.SpawnCreeps(21, 0.2f));                   
                }
            }

            public void Exit()
            {
                GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(false);
            }
        }
    }
}
