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
        public Type Type;
        [SerializeField]
        public float Value;
    }

    [Serializable]
    public enum Type
    {
        Cloth,
        Plate,
        Chainmail,
        Magic
    }
}