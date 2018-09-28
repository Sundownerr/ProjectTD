using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Base Tower")]
    public class TowerStats : Entity
    {
        public float damage, range, attackSpeed, triggerChance, critChance, critMultiplier, multicritCount;
        public float spellDamage, spellCritChance, spellCritMultiplier;
        public float mana, manaRegen;
        public float goldRatio, expRatio, itemDropRatio, itemQuialityRatio;
        public float buffDuration, debuffDuration;

        public List<TowerGradeStats> towerGradeStats;
        public List<float> gradeCosts;
        public List<float> damageToArmor;
    }
}