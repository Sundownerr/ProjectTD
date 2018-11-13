﻿using System;
using System.Collections.Generic;
using Game.Tower.Data.Stats;
using UnityEngine;

namespace Game.Systems
{
    public class TowerCreatingSystem 
    {      
        public event EventHandler AddedNewAvailableTower = delegate{};

        public TowerCreatingSystem() => GM.I.TowerCreatingSystem = this;      

        public void CreateRandomTower()
        {
            // if (GM.Instance.PlayerData.StartTowerRerollCount > 0)
            // {
            //     GM.Instance.AvailableTowerList.Clear();
            //     GM.Instance.PlayerData.StartTowerRerollCount--;
            // }
            
            var elementLevelList = GM.I.PlayerData.ElementLevelList;
            var dataBaseElementList = GM.I.TowerDataBase.AllTowerList.ElementsList;

            for (int lvldUpElementId = 0; lvldUpElementId < elementLevelList.Count; lvldUpElementId++)
                if (elementLevelList[lvldUpElementId] > 0)
                    for (int dataBaseElementId = 0; dataBaseElementId < dataBaseElementList.Count; dataBaseElementId++)
                        if (dataBaseElementId == lvldUpElementId) 
                            GetTower(lvldUpElementId);  
        
            void GetTower(int elementId)
            {
                var allTowerList = GM.I.TowerDataBase.AllTowerList;     
                var elementList = allTowerList.ElementsList;

                for (int i = 0; i < elementList[elementId].RarityList.Count; i++)
                    for (int j = 0; j < elementList[elementId].RarityList[i].TowerList.Count; j++)           
                        if (elementList[elementId].RarityList[i].TowerList[j].WaveLevel >= GM.I.WaveSystem.WaveNumber)
                        {
                            GM.I.AvailableTowerList.Add(elementList[elementId].RarityList[i].TowerList[j]);    
                            GM.I.BuildUISystem.AddTowerButton(elementList[elementId].RarityList[i].TowerList[j]);                               
                        }                   
            }       
            AddedNewAvailableTower?.Invoke(this, new EventArgs());                                    
        }
    }    
}