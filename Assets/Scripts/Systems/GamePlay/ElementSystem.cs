
using System;

namespace Game.Systems
{
    public class ElementSystem
    {
        public event EventHandler<int> LearnedElement = delegate{};

        private bool CheckCanLearn(int elementLevel)
        {
            var baseLearnCost   = 20;
            var levelLimit      = 15;
            var learnCost       = elementLevel + baseLearnCost;
            var isCanLearn      = elementLevel < levelLimit;
            var isLearnCostOk   = learnCost <= GM.I.PlayerData.MagicCrystals;

            if (!isCanLearn || !isLearnCostOk)
                return false;
           
            LearnedElement?.Invoke(this, learnCost);          

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
