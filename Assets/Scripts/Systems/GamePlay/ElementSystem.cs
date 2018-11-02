
namespace Game.System
{
    public class ElementSystem
    {
        public bool isFirstElementLearned;
        private int baseLearnCost, levelLimit;

        public ElementSystem()
        {
            baseLearnCost = 20;
            levelLimit = 15;

            GM.Instance.ElementSystem = this;
        }

        private bool CheckCanLearn(int elementLevel)
        {
            var learnCost = elementLevel + baseLearnCost;
            var isCanLearn      = elementLevel < levelLimit;
            var isLearnCostOk   = learnCost <= GM.Instance.PlayerData.MagicCrystals;

            if (!(isLearnCostOk && isCanLearn))
                return false;
            else
            {
                GM.Instance.ResourceSystem.AddMagicCrystal(-learnCost);

                var isButtonOk =
                    GM.Instance.BaseUISystem.GetTowerButton != null &&
                    !GM.Instance.BaseUISystem.GetTowerButton.gameObject.activeSelf;

                if (isButtonOk)
                    GM.Instance.BaseUISystem.GetTowerButton.gameObject.SetActive(true);
                
                return true;
            }              
        }

        public void LearnElement(int elementId)
        {
            if (CheckCanLearn(GM.Instance.PlayerData.ElementLevelList[elementId]))
                GM.Instance.PlayerData.ElementLevelList[elementId]++;
        }    
    }
}
