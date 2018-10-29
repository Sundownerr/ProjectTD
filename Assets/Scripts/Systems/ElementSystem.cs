
namespace Game.System
{
    public class ElementSystem
    {
        public bool isFirstElementLearned;
        private int baseLearnCost;

        public ElementSystem()
        {
            baseLearnCost = 20;

            GM.Instance.ElementSystem = this;
        }

        private bool CheckCanLearn(int elementLevel)
        {
            var learnCost = elementLevel + baseLearnCost;
            var isCanLearn = elementLevel < 15;

            if (learnCost <= GM.Instance.PlayerData.MagicCrystals && isCanLearn)
            {
                GM.Instance.ResourceSystem.AddMagicCrystal(-learnCost);

                var isButtonOk =
                    GM.Instance.BaseUISystem.GetTowerButton != null &&
                    !GM.Instance.BaseUISystem.GetTowerButton.gameObject.activeSelf;

                if (isButtonOk)
                    GM.Instance.BaseUISystem.GetTowerButton.gameObject.SetActive(true);
                
                return true;
            }
            else
                return false;     
        }

        public void LearnElement(int elementId)
        {
            if (CheckCanLearn(GM.Instance.PlayerData.ElementLevelList[elementId]))
                GM.Instance.PlayerData.ElementLevelList[elementId]++;
        }    
    }
}
