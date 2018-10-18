using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class PlayerDataSystem : ExtendedMonoBehaviour
    {
        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.PlayerDataSystem = this;
        }

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

        public bool CheckTowerLimit(int amount)
        {
            return (GM.Instance.PlayerData.CurrentTowerLimit + amount) <= GM.Instance.PlayerData.MaxTowerLimit;
        }

        private void LearnAstral()
        {

        }

        private void LearnDarkness()
        {

        }

        private void LearnIce()
        {

        }

        private void LearnIron()
        {

        }

        private void LearnStorm()
        {

        }

        private void LearnNature()
        {

        }

        private void LearnFire()
        {

        }
    }
}
