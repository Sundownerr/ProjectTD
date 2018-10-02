using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Entity.Tower
{
    public class TowerGradeStats : ScriptableObject
    {
        float damage, range, attackSpeed, triggerChance, critChance, critMultiplier, multicritCount;
        float spellDamage, spellCritChance, spellCritMultiplier;
        float mana, manaRegen;
        float goldRatio, expRatio, itemDropRatio, itemQuialityRatio;
        float buffDuration, debuffDuration;

    }
}