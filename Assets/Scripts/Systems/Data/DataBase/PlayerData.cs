using System;
using System.Collections.Generic;
using Game.Tower.Data.Stats;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Game.Systems;

namespace Game.Data
{   
    public interface IData
    {
        
    }
    [Serializable]
    public class PlayerData : IData
    {
        [SerializeField]
        public int MagicCrystals, Gold, CurrentTowerLimit, MaxTowerLimit, StartTowerRerollCount;

        [SerializeField]
        public List<int> ElementLevels; 
    }
}