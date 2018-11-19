
namespace Game.Systems
{
    public static class ElementSystem
    {
        private static bool CheckCanLearn(int elementLevel)
        {
            var baseLearnCost   = 20;
            var levelLimit      = 15;
            var learnCost       = elementLevel + baseLearnCost;
            var isCanLearn      = elementLevel < levelLimit;
            var isLearnCostOk   = learnCost <= GM.I.PlayerData.MagicCrystals;

            if (!isCanLearn || !isLearnCostOk)
                return false;
           
            GM.I.ResourceSystem.AddMagicCrystal(-learnCost);

            var isButtonOk =
                GM.I.BaseUISystem.GetTowerButton != null &&
                !GM.I.BaseUISystem.GetTowerButton.gameObject.activeSelf;

            GM.I.BaseUISystem.GetTowerButton.gameObject.SetActive(isButtonOk);         
            return true;                    
        }

        public static void LearnElement(int elementId)
        {
            if (CheckCanLearn(GM.I.PlayerData.ElementLevelList[elementId]))
            {
                GM.I.PlayerData.ElementLevelList[elementId]++;
                GM.I.ElementUISystem.UpdateUI();
            }
        }    
    }
}
