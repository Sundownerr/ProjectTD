using Game.Data.Entity.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class TowerCreatingSystem : ExtendedMonoBehaviour
    {
        private List<int> leveledElementList;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.TowerCreatingSystem = this;

            leveledElementList = new List<int>();
        }
        
        public void CreateRandomTower()
        {
            if (GM.Instance.PlayerData.StartTowerRerollCount > 0)
            {
                GM.Instance.AvailableTowerList.Clear();
                GM.Instance.PlayerData.StartTowerRerollCount--;
            }

            leveledElementList.Clear();

            var elementLevelList = GM.Instance.PlayerData.ElementLevelList;

            for (int i = 0; i < elementLevelList.Count; i++)
            {
                if (elementLevelList[i] > 0)
                {
                    leveledElementList.Add(i);                 
                }
            }

            for (int i = 0; i < leveledElementList.Count; i++)
            {
                for (int j = 0; j < GM.Instance.AllTowerData.AllTowerList.ElementsList.Count; j++)
                {
                    if (j == i)
                    {
                        var random = Random.Range(0, 2);

                        if (random == 1)
                        {
                            GetTower(leveledElementList[i]);
                        }
                        
                    }
                }
            }
        }

        private void GetTower(int Id)
        {
            var allTowerList = GM.Instance.AllTowerData.AllTowerList;

           

            for (int i = 0; i < allTowerList.ElementsList[Id].RarityList.Count; i++)
            {
                for (int j = 0; j < allTowerList.ElementsList[Id].RarityList[i].TowerList.Count; j++)
                {
                    if (allTowerList.ElementsList[Id].RarityList[i].TowerList[j].Wave >= GM.Instance.WaveSystem.WaveCount)
                    {
                        GM.Instance.AvailableTowerList.Add(allTowerList.ElementsList[Id].RarityList[i].TowerList[j]);
                        GM.Instance.BuildUISystem.UpdateAvailableElement();
                    }
                }
            }
        }

        
    }
}