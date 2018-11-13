
namespace Game.Systems
{
    public class ElementSystem
    {
        private int baseLearnCost, levelLimit;

        public ElementSystem()
        {
            baseLearnCost = 20;
            levelLimit = 15;

            GM.I.ElementSystem = this;
        }

        private bool CheckCanLearn(int elementLevel)
        {
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

        public void LearnElement(int elementId)
        {
            if (CheckCanLearn(GM.I.PlayerData.ElementLevelList[elementId]))
            {
                GM.I.PlayerData.ElementLevelList[elementId]++;
                GM.I.ElementUISystem.UpdateUI();
            }
        }    
    }
}
