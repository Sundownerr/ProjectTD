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

            GameManager.Instance.WaveSystem = this;
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.BaseUISystem.IsWaveStarted)
            {
                if (!isCreepSpawning)
                {
                    StartCoroutine(SpawnCreeps(2, 0.1f));
                }

                if (isCreepsSpawned)
                {
                    if (GameManager.Instance.CreepList.Count == 0)
                    {
                        isCreepsSpawned = false;
                        isCreepSpawning = false;
                        GameManager.Instance.BaseUISystem.IsWaveStarted = false;
                        GameManager.Instance.BaseUISystem.StartWaveButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            var spawnedCreepCount = 0;         

            while (spawnedCreepCount < needToSpawnCount)
            {
                Instantiate(GameManager.Instance.CreepPrefab);
                
                isCreepSpawning = true;

                spawnedCreepCount++;

                yield return new WaitForSeconds(spawnDelay);
            }

            isCreepsSpawned = true;            
        }
    }
}
