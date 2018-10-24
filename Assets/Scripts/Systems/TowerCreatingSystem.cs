using Game.Data.Entity.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class TowerCreatingSystem : ExtendedMonoBehaviour
    {
        private List<int> LeveledElementList;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.TowerCreatingSystem = this;

            LeveledElementList = new List<int>();
        }
        
        public void CreateRandomTower()
        {
            LeveledElementList.Clear();

            var elementLevelList = GM.Instance.PlayerData.ElementLevelList;

            for (int i = 0; i < elementLevelList.Count; i++)
            {
                if (elementLevelList[i] > 0)
                {
                    LeveledElementList.Add(i);                 
                }
            }

            for (int i = 0; i < LeveledElementList.Count; i++)
            {
                for (int j = 0; j < GM.Instance.AllTowerData.AllTowerList.Count; j++)
                {
                    if (j == i)
                    {
                        var random = Random.Range(0, 1);

                        if (random == 1)
                        {
                            GetTower(j);
                        }                      
                    }
                }
            }
        }

        private void GetTower(int elementId)
        {
            var allTowerList = GM.Instance.AllTowerData.AllTowerList;

            for (int i = 0; i < allTowerList[elementId].Rarities.Count; i++)
            {
                for (int j = 0; j < allTowerList[elementId].Rarities[i].Towers.Count; j++)
                {
                    if (allTowerList[elementId].Rarities[i].Towers[j].Wave >= GM.Instance.WaveSystem.WaveCount)
                    {
                        GM.Instance.AvailableTowerList.Add(allTowerList[elementId].Rarities[i].Towers[j]);
                    }
                }
            }
        }
    }
}