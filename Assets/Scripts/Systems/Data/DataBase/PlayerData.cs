using System;
using System.Collections.Generic;
using Game.Tower.Data.Stats;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Game.Systems;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Data/Player Data")]
    
    [Serializable]
    public class PlayerData
    {
        [SerializeField]
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit, StartTowerRerollCount;

        [SerializeField]
        public List<int> ElementLevelList; 
    }
}