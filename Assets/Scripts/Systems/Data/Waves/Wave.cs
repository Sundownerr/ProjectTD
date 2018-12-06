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
        public List<Trait> WaveTraits { get => waveTraits; set => waveTraits = value; }
        public List<Ability> WaveAbilities { get => waveAbilities; set => waveAbilities = value; }

        [SerializeField]
        public List<CreepData> CreepTypes;
              
        private List<Ability> waveAbilities;
        private List<Trait> waveTraits;

    }
}