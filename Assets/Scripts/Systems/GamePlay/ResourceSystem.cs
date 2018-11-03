
namespace Game.Systems
{
    public class ResourceSystem
    {
        public ResourceSystem() => GM.Instance.ResourceSystem = this;      

        public void AddMagicCrystal(int amount)
        {
            GM.Instance.PlayerData.MagicCrystals += amount;
            GM.Instance.BaseUISystem.UpdateResourceValues();
        }

        public void AddGold(int amount)
        {
            GM.Instance.PlayerData.Gold += amount;
            GM.Instance.BaseUISystem.UpdateResourceValues();
        }

        public void AddTowerLimit(int amount)
        {
            GM.Instance.PlayerData.CurrentTowerLimit += amount;
            GM.Instance.BaseUISystem.UpdateResourceValues();
        }

        public bool CheckHaveResources(int towerLimitCost, int goldCost, int magicCrystalCost)
        {
            var isTowerLimitOk = 
                (GM.Instance.PlayerData.CurrentTowerLimit + towerLimitCost) <= GM.Instance.PlayerData.MaxTowerLimit;

            var isGoldCostOk = 
                goldCost <= GM.Instance.PlayerData.Gold;

            var isMagicCrystalCostOk = 
                magicCrystalCost <= GM.Instance.PlayerData.MagicCrystals; 

            return isTowerLimitOk && isGoldCostOk && isMagicCrystalCostOk;
        }
    }
}
