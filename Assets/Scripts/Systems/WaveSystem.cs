using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{
    public class WaveSystem : MonoBehaviour
    {
        private bool isCreepsStartSpawning, isCreepsSpawned;

        private IEnumerator SpawnCreeps(int needToSpawnCount, float spawnDelay)
        {
            var spawnedCreepCount = 0;         

            while (spawnedCreepCount < needToSpawnCount)
            {
                Instantiate(GameManager.Instance.CreepPrefab);

                isCreepsStartSpawning = true;

                spawnedCreepCount++;

                yield return new WaitForSeconds(spawnDelay);
            }

            isCreepsSpawned = true;            
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.UISystem.IsWaveStarted)
            {
                if (!isCreepsStartSpawning)
                {
                    StartCoroutine(SpawnCreeps(25, 0.1f));
                }

                if (isCreepsSpawned)
                {
                    if (GameManager.Instance.CreepList.Count == 0)
                    {
                        isCreepsSpawned = false;
                        isCreepsStartSpawning = false;                       
                        GameManager.Instance.UISystem.IsWaveStarted = false;
                        GameManager.Instance.UISystem.StartWaveButton.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
