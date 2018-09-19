using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Tower", menuName ="Base Tower")]
public class TowerStats : ScriptableObject
{
    public string towerName;

    public float damage, range, attackSpeed, triggerChance, critChance, critMultiplier, multicritCount;
    public float spellDamage, spellCritChance, spellCritMultiplier;
    public float mana, manaRegen;
    public float goldRatio, expRatio, itemDropRatio, itemQuialityRatio;
    public float buffDuration, debuffDuration;
    
    public List<TowerGradeStats> towerGradeStats;
    public List<float> gradeCosts;
    public List<float> damageToArmor;
}
