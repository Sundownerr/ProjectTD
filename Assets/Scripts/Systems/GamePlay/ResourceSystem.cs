
using System;

namespace Game.Systems
{
    public class ResourceSystem
    {
        public event EventHandler ResourcesChanged = delegate{};

        public ResourceSystem() 
        {
            GM.I.ResourceSystem = this;                
        }

        public void SetSystem()
        {
            GM.I.TowerPlaceSystem.TowerDeleted += OnTowerDeleted;
            GM.I.TowerPlaceSystem.TowerCreated += OnTowerCreated;
            GM.I.PlayerInputSystem.TowerSold += OnTowerDeleted;
        }

        private void OnTowerDeleted(object sender, TowerEventArgs e)
        {
            AddGold(e.Stats.GoldCost);
            AddTowerLimit(-e.Stats.TowerLimit);
        }

        private void OnTowerCreated(object sender, TowerEventArgs e)
        {
            AddGold(-e.Stats.GoldCost);
            AddTowerLimit(e.Stats.TowerLimit);
        }

        public void AddMagicCrystal(int amount)
        {
            GM.I.PlayerData.MagicCrystals += amount;
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public void AddGold(int amount)
        {
            GM.I.PlayerData.Gold += amount;
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public void AddTowerLimit(int amount)
        {
            GM.I.PlayerData.CurrentTowerLimit += amount;
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public bool CheckHaveResources(int towerLimitCost, int goldCost, int magicCrystalCost) =>
            (GM.I.PlayerData.CurrentTowerLimit + towerLimitCost) <= GM.I.PlayerData.MaxTowerLimit && 
            goldCost <= GM.I.PlayerData.Gold && 
            magicCrystalCost <= GM.I.PlayerData.MagicCrystals;    
    }
}
