
using System;

namespace Game.Systems
{
    public class ResourceSystem
    {
        public event EventHandler ResourcesChanged = delegate{};

        public ResourceSystem() => GM.Instance.ResourceSystem = this;      

        public void AddMagicCrystal(int amount)
        {
            GM.Instance.PlayerData.MagicCrystals += amount;
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public void AddGold(int amount)
        {
            GM.Instance.PlayerData.Gold += amount;
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public void AddTowerLimit(int amount)
        {
            GM.Instance.PlayerData.CurrentTowerLimit += amount;
            ResourcesChanged?.Invoke(this, new EventArgs());
        }

        public bool CheckHaveResources(int towerLimitCost, int goldCost, int magicCrystalCost) =>
            (GM.Instance.PlayerData.CurrentTowerLimit + towerLimitCost) <= GM.Instance.PlayerData.MaxTowerLimit && 
            goldCost <= GM.Instance.PlayerData.Gold && 
            magicCrystalCost <= GM.Instance.PlayerData.MagicCrystals;    
    }
}
