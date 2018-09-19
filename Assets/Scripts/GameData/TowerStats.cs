using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : ScriptableObject
{
    float damage, range, attackSpeed, triggerChance, critChance, critMultiplier, multicritCount;
    float spellDamage, spellCritChance, spellCritMultiplier;
    float mana, manaRegen;
    float goldRatio, expRatio, itemDropRatio, itemQuialityRatio;
    float buffDuration, debuffDuration;
    List<float> gradeCosts;
    List<float> damageToArmor;
}
