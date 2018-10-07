using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Base Tower")]
    public class TowerStats : Entity
    {
        

        public int Id, Exp, Level;
        public int Damage, Range, CritMultiplier, SpellCritMultiplier, Mana,  MulticritCount;
        public float AttackSpeed, TriggerChance, CritChance, ManaRegen;
        public float SpellDamage, SpellCritChance;
  
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio;
        public float BuffDuration, DebuffDuration;
        
        public List<TowerGradeStats> TowerGradeStatList;
        [Expandable]
        public List<Ability> TowerAbilityList;
        public List<float> GradeCostList;
        public List<float> DamageToArmorList;

    }
}