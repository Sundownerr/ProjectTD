using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class ResourceSystem : ExtendedMonoBehaviour
    {
        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.ResourceSystem = this;
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
    }
}
