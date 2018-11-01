using Game.Data.Entity.Tower;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Tower.Data.Stats
{
    [Serializable]
    public class Element
    {
        [SerializeField]
        public List<Rarity> RarityList;

        [SerializeField]
        public string Name;
    }

    [Serializable]
    public class Rarity
    {
        [SerializeField]
        public List<TowerData> TowerList;

        [SerializeField]
        public string Name;

        public Rarity(List<TowerData> towerList, string name)
        {
            TowerList = towerList;
            Name = name;
        }
    }

    [Serializable]
    public class ElementList
    {
        [SerializeField]
        public List<Element> ElementsList;
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
        Astral = 0,
        Darkness = 1,
        Ice = 2,
        Iron = 3,
        Storm = 4,
        Nature = 5,
        Fire = 6
    }

    [Serializable]
    public enum RarityType
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Unique = 3,
    }
}
