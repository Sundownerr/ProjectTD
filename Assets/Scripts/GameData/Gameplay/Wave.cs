using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using UnityEngine;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Data/Wave")]
    [Serializable]
    public class Wave : ScriptableObject
    {
         [SerializeField]
        public List<CreepType> CreepTypeList;       
    }
}