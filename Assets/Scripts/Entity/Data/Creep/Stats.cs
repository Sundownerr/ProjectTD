﻿using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Creep.Data
{
    [Serializable]
    public struct Armor
    {
        [SerializeField]
        private ArmorType type;

        [SerializeField]
        private float value;

        public ArmorType Type { get => type; set => type = value; }
        public float Value { get => value; set => this.value = value; }

        [Serializable]
        public enum ArmorType
        {
            Cloth,
            Plate,
            Chainmail,
            Magic
        }
    }

    [Serializable]
    public enum RaceType
    {
        Humanoid    = 0,
        Magical     = 1,
        Undead      = 2, 
        Nature      = 3
    }

    

    [Serializable]
    public class Race
    {
        [SerializeField, Expandable]
        public List<CreepData> Creeps;

        public Race()
        {
            Creeps = new List<CreepData>();
        }           
    }
}