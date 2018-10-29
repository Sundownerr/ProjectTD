using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
    
    public class PlayerData : ScriptableObject
    {
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit;
        public int StartTowerRerollCount;

        public List<int> ElementLevelList;

        private void Awake()
        {
            ElementLevelList = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                ElementLevelList.Add(0);
            }                

            MaxTowerLimit = 500;
            StartTowerRerollCount = 3;
            MagicCrystals = 100;
        }
    }
}