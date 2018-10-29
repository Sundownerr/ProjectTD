using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Data/Wave")]
    public class Wave : ScriptableObject
    {
        public List<Creep.CreepSystem> CreepList;
        public int RaceId, ArmorId;
    }
}