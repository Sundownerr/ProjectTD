using System.Collections.Generic;

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
            // if (GM.Instance.PlayerData.StartTowerRerollCount > 0)
            // {
            //     GM.Instance.AvailableTowerList.Clear();
            //     GM.Instance.PlayerData.StartTowerRerollCount--;
            // }

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
            var elementList = allTowerList.ElementsList;

            for (int i = 0; i < elementList[id].RarityList.Count; i++)
                for (int j = 0; j < elementList[id].RarityList[i].TowerList.Count; j++)           
                    if (elementList[id].RarityList[i].TowerList[j].WaveLevel >= GM.Instance.WaveSystem.WaveNumber)
                    {
                        GM.Instance.AvailableTowerList.Add(elementList[id].RarityList[i].TowerList[j]);    
                        GM.Instance.BuildUISystem.AddTowerButton(elementList[id].RarityList[i].TowerList[j]);                 
                    }      

            GM.Instance.BuildUISystem.UpdateAvailableElement();
            GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.BuildUISystem.ChoosedElement);
        }
    }
}