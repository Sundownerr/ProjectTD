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
        public List<Trait> WaveTraitList { get => waveTraitList; set => waveTraitList = value; }
        public List<Ability> WaveAbilityList { get => waveAbilityList; set => waveAbilityList = value; }

        [SerializeField]
        public List<CreepData> CreepTypeList;
              
        private List<Ability> waveAbilityList;
        private List<Trait> waveTraitList;

    }
}