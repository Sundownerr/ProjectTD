using System.Collections.Generic;
using Game.Creep.Data;
using UnityEngine;
using System;
using Game.Creep;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Data/Wave")]
    [Serializable]
    public class Wave : ScriptableObject
    {
        [SerializeField]
        public List<CreepData> CreepTypeList;       
    }
}