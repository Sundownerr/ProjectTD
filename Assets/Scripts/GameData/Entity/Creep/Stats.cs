using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Data.Entity.Creep
{
    [Serializable]
    public struct Armor
    {
        [SerializeField]
        public ArmorType Type;
        [SerializeField]
        public float Value;

        [Serializable]
        public enum ArmorType
        {
            Cloth,
            Plate,
            Chainmail,
            Magic
        }
    }

    public enum RaceType
    {
        Humanoid = 0,
        Magical = 1,
        Undead = 2, 
        Nature = 3
    }

}