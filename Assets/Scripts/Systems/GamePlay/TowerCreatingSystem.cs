using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class TowerCreatingSystem 
    {      
        public TowerCreatingSystem() 
        {           
            GM.Instance.TowerCreatingSystem = this;
        }

        public void CreateRandomTower()
        {
            if (GM.Instance.PlayerData.StartTowerRerollCount > 0)
            {
                GM.Instance.AvailableTowerList.Clear();
                GM.Instance.PlayerData.StartTowerRerollCount--;
            }

            var leveledElementList = new List<int>();
            var elementLevelList = GM.Instance.PlayerData.ElementLevelList;

            for (int i = 0; i < elementLevelList.Count; i++)
                if (elementLevelList[i] > 0)
                    leveledElementList.Add(i);

            for (int i = 0; i < leveledElementList.Count; i++)
                for (int j = 0; j < GM.Instance.TowerDataBase.AllTowerList.ElementsList.Count; j++)
                    if (j == i) 
                        GetTower(leveledElementList[i]);                                           
        }

        private void GetTower(int id)
        {
            var allTowerList = GM.Instance.TowerDataBase.AllTowerList;        

            for (int i = 0; i < allTowerList.ElementsList[id].RarityList.Count; i++)
                for (int j = 0; j < allTowerList.ElementsList[id].RarityList[i].TowerList.Count; j++)
                    if (allTowerList.ElementsList[id].RarityList[i].TowerList[j].WaveLevel >= GM.Instance.WaveSystem.WaveNumber)
                    {
                        GM.Instance.AvailableTowerList.Add(allTowerList.ElementsList[id].RarityList[i].TowerList[j]);
                        GM.Instance.BuildUISystem.UpdateAvailableElement();
                    }       
            
            GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.BuildUISystem.ChoosedElement);
        }
    }
}