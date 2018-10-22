using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class ForceSystem : ExtendedMonoBehaviour
    {
        public bool isFirstElementLearned;
        private int baseLearnCost;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            baseLearnCost = 20;

            GM.Instance.ForceSystem = this;
        }

        private bool CheckCanLearn(int elementLevel)
        {
            var learnCost = elementLevel + baseLearnCost;

            if (learnCost <= GM.Instance.PlayerData.MagicCrystals)
            {
                GM.Instance.ResourceSystem.AddMagicCrystal(-learnCost);

                var isButtonOk =
                    GM.Instance.BaseUISystem.GetTowerButton != null &&
                    !GM.Instance.BaseUISystem.GetTowerButton.gameObject.activeSelf;

                if (isButtonOk)
                {
                    GM.Instance.BaseUISystem.GetTowerButton.gameObject.SetActive(true);
                }
                
                return true;
            }
            else
            {
                Debug.Log("not enough mc");
                return false;
            }         
        }

        public void LearnElement(string elementName)
        {
            if (CheckCanLearn(GM.Instance.PlayerData.ElementLevelList[elementName]))
            {
                GM.Instance.PlayerData.ElementLevelList[elementName]++;
            }
        }    
    }

}
