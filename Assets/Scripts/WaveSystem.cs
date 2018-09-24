using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private List<GameObject> creeps;
    private List<Creep> creepData;
    private bool creepsSpawned;
    private int creepsAmountSpawned, creepsAmountNotSpawned, spawnDelay;
    
    private void SpawnCreeps(int amount, float delay)
    {
        if (spawnDelay < 19)
        {
            spawnDelay++;

            if (spawnDelay == 1)
            {
                if (creepsAmountSpawned < amount)
                {
                    creeps.Add(Instantiate(GameManager.Instance.CreepPrefab));
                    creepData.Add(creeps[creeps.Count - 1].GetComponent<Creep>());
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
    
    private void Start ()
    {       
        creeps = new List<GameObject>();
        creepData = new List<Creep>();
        creepsAmountNotSpawned = 5;
	}	

	private void Update ()
    {
        
        if (GameManager.Instance.UISystem.IsWaveStarted)
        {
            if (!creepsSpawned)
            {
                SpawnCreeps(creepsAmountNotSpawned, 0.5f);
            }

            if (creepsSpawned)
            {
                for (int i = 0; i < creepData.Count; i++)
                {
                    if(creepData[i].ReachedLastWaypoint)
                    {
                        creeps.RemoveAt(i);
                        creepData.RemoveAt(i);
                    }
                }

                if(creeps.Count == 0)
                {
                    creepsAmountSpawned = 0;
                    creepsSpawned = false;
                    GameManager.Instance.UISystem.IsWaveStarted = false;            
                }
            }
        }
	}
}
