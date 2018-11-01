using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Data.Entity.Tower
{
    [Serializable]
    public struct Damage 
    {
        [SerializeField]
        public Type Type;
        [SerializeField]
        public float Value;      
    }

    [Serializable]
    public enum Type
    {
        Spell,
        Decay,
        Energy,
        Physical,
        Elemental
    }
}