using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Tower.Data.Stats
{
    
    [Serializable]
    public class ElementList
    {
        [SerializeField]
        public List<Element> Elements;
    }

    [Serializable]
    public class Element
    {
        [SerializeField]
        public List<Rarity> Rarities;

        [SerializeField]
        public string Name;

        public Element(string name)
        {
            Name = name;        
            Rarities = new List<Rarity>
            {
                new Rarity(RarityType.Common.ToString()),
                new Rarity(RarityType.Uncommon.ToString()),
                new Rarity(RarityType.Rare.ToString()),
                new Rarity(RarityType.Unique.ToString())
            };
        }           
    }

    [Serializable]
    public class Rarity
    {
        [SerializeField]
        public List<TowerData> Towers;

        [SerializeField]
        public string Name;

        public Rarity(string name) 
        {
            Name = name;          
            Towers = new List<TowerData>();          
        }
    }

    [Serializable]
    public struct Damage
    {
        [SerializeField]
        public Type ThisType;

        [SerializeField]
        public float Value;

        [Serializable]
        public enum Type
        {
            Spell,
            Decay,
            Energy,
            Physical,
            Elemental,
        }
    }

    [Serializable]
    public enum ElementType
    {
        Astral      = 0,
        Darkness    = 1,
        Ice         = 2,
        Iron        = 3,
        Storm       = 4,
        Nature      = 5,
        Fire        = 6,
    }

    [Serializable]
    public enum RarityType
    {
        Common      = 0,
        Uncommon    = 1,
        Rare        = 2,
        Unique      = 3,
    }
}
