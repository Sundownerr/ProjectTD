using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.System
{
    public class WaveSystem : MonoBehaviour
    {

        private bool creepsSpawned;
        private int creepsAmountSpawned, creepsAmountNotSpawned, spawnDelay;

        private void SpawnCreeps(int amount, float delay)
        {
            if (spawnDelay < delay)
            {
                spawnDelay++;

                if (spawnDelay == 1)
                {
                    if (creepsAmountSpawned < amount)
                    {
                        Instantiate(GameManager.Instance.CreepPrefab);

                        creepsAmountSpawned++;
                    }
                    else
                    {
                        creepsSpawned = true;
                    }
                }
            }
            else
            {
                spawnDelay = 0;
            }
        }

        private void Start()
        {
            creepsAmountNotSpawned = 15;
        }

        private void Update()
        {
            if (GameManager.Instance.UISystem.IsWaveStarted)
            {
                if (!creepsSpawned)
                {
                    SpawnCreeps(creepsAmountNotSpawned, 30f);
                }

                if (creepsSpawned)
                {
                    if (GameManager.Instance.CreepList.Count == 0)
                    {
                        creepsAmountSpawned = 0;
                        creepsSpawned = false;
                        GameManager.Instance.UISystem.IsWaveStarted = false;
                    }
                }
            }
        }
    }
}
