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
        public int StartTowerRerollCount;

        public Dictionary<string, int> ElementLevelList = new Dictionary<string, int>
        {
            ["Astral"] = 0,
            ["Darkness"] = 0,
            ["Ice"] = 0,
            ["Iron"] = 0,
            ["Storm"] = 0,
            ["Nature"] = 0,
            ["Fire"] = 0
        };

        private void Awake()
        {          
            MaxTowerLimit = 500;
            StartTowerRerollCount = 3;
            MagicCrystals = 70;
        }
    }
}