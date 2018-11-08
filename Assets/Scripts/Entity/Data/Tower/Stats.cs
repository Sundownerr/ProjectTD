using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Tower.Data.Stats
{
    
    [Serializable]
    public class ElementList
    {
        [SerializeField]
        public List<Element> ElementsList;
    }

    [Serializable]
    public class Element
    {
        [SerializeField]
        public List<Rarity> RarityList;

        [SerializeField]
        public string Name;

        public Element(string name)
        {
            Name = name;        
            RarityList = new List<Rarity>
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
        public List<TowerData> TowerList;

        [SerializeField]
        public string Name;

        public Rarity(string name) 
        {
            Name = name;          
            TowerList = new List<TowerData>();          
        }
    }

    [Serializable]
    public struct Damage
    {
        [SerializeField]
        public DamageType Type;

        [SerializeField]
        public float Value;

        [Serializable]
        public enum DamageType
        {
            Spell,
            Decay,
            Energy,
            Physical,
            Elemental
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
        Fire        = 6
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
