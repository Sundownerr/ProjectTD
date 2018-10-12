using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{
    public class WaveSystem : ExtendedMonoBehaviour
    {
        private bool isCreepSpawning, isCreepsSpawned;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.WaveSystem = this;
        }

        private void LateUpdate()
        {
            if (GM.Instance.BaseUISystem.IsWaveStarted)
            {
                if (!isCreepSpawning)
                {
                    StartCoroutine(SpawnCreeps(2, 0.1f));
                }

                if (isCreepsSpawned)
                {
                    if (GM.Instance.CreepList.Count == 0)
                    {
                        isCreepsSpawned = false;
                        isCreepSpawning = false;
                        GM.Instance.BaseUISystem.IsWaveStarted = false;
                        GM.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            var spawnedCreepCount = 0;         

            while (spawnedCreepCount < needToSpawnCount)
            {
                Instantiate(GM.Instance.CreepPrefab);
                
                isCreepSpawning = true;

                spawnedCreepCount++;

                yield return new WaitForSeconds(spawnDelay);
            }

            isCreepsSpawned = true;            
        }
    }
}
