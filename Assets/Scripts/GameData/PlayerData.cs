using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "PlayerData")]
    
    public class PlayerData : ScriptableObject
    {
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit;
        public int StartTowerRerollCount;


        public List<int> ElementLevelList;

        private void Awake()
        {
            ElementLevelList = new List<int>();
            MaxTowerLimit = 500;
            StartTowerRerollCount = 3;
            MagicCrystals = 100;
        }
    }
}