using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "PlayerData")]

    public class PlayerData : ScriptableObject
    {
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit;
        public int AstralLevel, DarknessLevel, IceLevel, IronLevel, StormLevel, NatureLevel, FireLevel;

        private void Awake()
        {
            MaxTowerLimit = 30;
        }
    }
}