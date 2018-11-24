
using System;
using Game.Creep;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
    public class ResourceSystem
    {
        public event EventHandler ResourcesChanged = delegate{};

        private enum ResourceType
        {
            Gold,
            MagicCrystal,
            TowerLimit
        }

        public ResourceSystem() 
        {
            GM.I.ResourceSystem = this;                
        }

        public void SetSystem()
        {
            GM.I.TowerPlaceSystem.TowerDeleted += OnTowerDeleted;
            GM.I.TowerPlaceSystem.TowerCreated += OnTowerCreated;
            GM.I.PlayerInputSystem.TowerSold += OnTowerDeleted;
            GM.I.ElementSystem.LearnedElement += OnElementLearned;
            GM.I.WaveSystem.AllWaveCreepsKilled += OnAllCreepsKilled;
        }
      
        private void OnTowerDeleted(object sender, TowerEventArgs e)
        {
            AddResource(ResourceType.Gold, e.Stats.GoldCost);
            AddResource(ResourceType.TowerLimit, -e.Stats.TowerLimit);
        }

        private void OnTowerCreated(object sender, TowerEventArgs e)
        {
            AddResource(ResourceType.Gold, -e.Stats.GoldCost);
            AddResource(ResourceType.TowerLimit, e.Stats.TowerLimit);
        }

        public void OnCreepDied(object sender, CreepData creep) => AddResource(ResourceType.Gold, creep.Gold);

        private void OnElementLearned(object sender, int learnCost) => AddResource(ResourceType.MagicCrystal, -learnCost);       

        private void OnAllCreepsKilled(object sender, EventArgs e) => AddResource(ResourceType.MagicCrystal, 5);       

        private void AddResource(ResourceType type, int amount)
        {
            if(type == ResourceType.Gold)
                GM.I.PlayerData.Gold += amount;

            else if (type == ResourceType.MagicCrystal)
                GM.I.PlayerData.MagicCrystals += amount;

            else if (type == ResourceType.TowerLimit)
                GM.I.PlayerData.CurrentTowerLimit += amount;

            DataLoadingSystem.Save<PlayerData>(GM.I.PlayerData);
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public bool CheckHaveResources(int towerLimitCost, int goldCost, int magicCrystalCost) =>
            (GM.I.PlayerData.CurrentTowerLimit + towerLimitCost) <= GM.I.PlayerData.MaxTowerLimit && 
            goldCost <= GM.I.PlayerData.Gold && 
            magicCrystalCost <= GM.I.PlayerData.MagicCrystals;    
    }
}
