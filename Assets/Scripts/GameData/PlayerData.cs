using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Player Data", menuName = "PlayerData")]

    public class PlayerData : ScriptableObject
    {
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit;
        public int AstralLevel, DarknessLevel, IceLevel, IronLevel, StormLevel, NatureLevel, FireLevel;

        private int baseLearnCost;

        private void Awake()
        {
            MagicCrystals = 1;
            Gold = 1;
            CurrentTowerLimit = 0;
            MaxTowerLimit = 30;
            baseLearnCost = 15;
        }
    }
}