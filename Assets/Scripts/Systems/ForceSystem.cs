using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class ForceSystem : ExtendedMonoBehaviour
    {

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
                return true;
            }
            else
            {
                Debug.Log("not enough mc");
                return false;
            }         
        }

        public void LearnAstral()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.AstralLevel))
            {
                GM.Instance.PlayerData.AstralLevel++;
            }
        }

        public void LearnDarkness()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.DarknessLevel))
            {
                GM.Instance.PlayerData.DarknessLevel++;
            }
        }

        public void LearnIce()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.IceLevel))
            {
                GM.Instance.PlayerData.IceLevel++;
            }
        }

        public void LearnIron()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.IronLevel))
            {
                GM.Instance.PlayerData.IronLevel++;
            }
        }

        public void LearnStorm()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.StormLevel))
            {
                GM.Instance.PlayerData.StormLevel++;
            }
        }

        public void LearnNature()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.NatureLevel))
            {
                GM.Instance.PlayerData.NatureLevel++;
            }
        }

        public void LearnFire()
        {
            if (CheckCanLearn(GM.Instance.PlayerData.FireLevel))
            {
                GM.Instance.PlayerData.FireLevel++;
            }
        }
    }

}
