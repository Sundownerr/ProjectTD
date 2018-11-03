using System;
using System.Collections;
using System.Collections.Generic;
using Game.Tower.Data.Stats;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
    
    [Serializable]
    public class PlayerData : ScriptableObject
    {
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit;
        public int StartTowerRerollCount;

        [SerializeField]
        public List<int> ElementLevelList;

        private void Awake()
        {
            if(ElementLevelList == null)
            {
                ElementLevelList = new List<int>();
                var elementAmount = Enum.GetValues(typeof(ElementType)).Length;

                for (int i = 0; i < elementAmount; i++)
                    ElementLevelList.Add(0);         
            } 

            MaxTowerLimit = 500;
            StartTowerRerollCount = 3;
            MagicCrystals = 100;
        }
    }
}