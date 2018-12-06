using System;
using System.Collections.Generic;
using Game.Tower.Data.Stats;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Systems
{
    public class TowerCreatingSystem 
    {      
        public event EventHandler AddedNewAvailableTower = delegate{};

        public TowerCreatingSystem() => GM.I.TowerCreatingSystem = this;   

        public void SetSystem()
        {
            GM.I.TowerPlaceSystem.TowerDeleted += OnTowerDeleted;
        }   

        private void OnTowerDeleted(object sender, TowerEventArgs e) 
        {
            var towerFromDB = 
                GM.I.TowerDataBase.AllTowers.
                Elements[(int)e.Stats.Element].
                Rarities[(int)e.Stats.Rarity].Towers.Find(tower => tower.CompareId(e.Stats.Id));

            GM.I.AvailableTowers.Add(towerFromDB);                    
        }

        public void CreateRandomTower()
        {
            // if (GM.Instance.PlayerData.StartTowerRerollCount > 0)
            // {
            //     GM.Instance.AvailableTowerList.Clear();
            //     GM.Instance.PlayerData.StartTowerRerollCount--;
            // }
            
            var elementLevels = GM.I.PlayerData.ElementLevels;
            var dataBaseElements = GM.I.TowerDataBase.AllTowers.Elements;

            for (int lvldUpElementId = 0; lvldUpElementId < elementLevels.Count; lvldUpElementId++)
                if (elementLevels[lvldUpElementId] > 0)
                    for (int dbElementId = 0; dbElementId < dataBaseElements.Count; dbElementId++)
                        if (dbElementId == lvldUpElementId) 
                            GetTower(lvldUpElementId);  

            AddedNewAvailableTower?.Invoke(this, new EventArgs());  

            #region  Helper functions

            void GetTower(int elementId)
            {  
                var elements = GM.I.TowerDataBase.AllTowers.Elements;

                for (int i = 0; i < elements[elementId].Rarities.Count; i++)
                    for (int j = 0; j < elements[elementId].Rarities[i].Towers.Count; j++)           
                        if (elements[elementId].Rarities[i].Towers[j].WaveLevel >= GM.I.WaveSystem.WaveNumber)
                        {
                            GM.I.AvailableTowers.Add(elements[elementId].Rarities[i].Towers[j]);    
                            GM.I.BuildUISystem.AddTowerButton(elements[elementId].Rarities[i].Towers[j]);    
                        }                   
            }      

            #endregion                                            
        }
    }    
}