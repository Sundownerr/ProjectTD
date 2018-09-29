using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{
    public class WaveSystem : MonoBehaviour
    {
        private bool isCreepsStartSpawning;

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            var spawnedCreepCount = 0;

            isCreepsStartSpawning = true;

            while (spawnedCreepCount < needToSpawnCount)
            {
                Instantiate(GameManager.Instance.CreepPrefab);

                spawnedCreepCount++;

                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.UISystem.IsWaveStarted)
            {
                if (!isCreepsStartSpawning)
                {
                    StartCoroutine(SpawnCreeps(5, 0.1f));
                }

                if (isCreepsStartSpawning)
                {
                    if (GameManager.Instance.CreepList.Count == 0)
                    {
                        isCreepsStartSpawning = false;
                        GameManager.Instance.UISystem.IsWaveStarted = false;
                    }
                }
            }
        }
    }
}
