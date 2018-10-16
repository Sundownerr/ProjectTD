using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Base Tower")]
    public class TowerData : Entity
    {        
        public int Id, Exp, Level, TowerLimit, MagicCrystalsRequirement, ElementId, RarityId, DamageTypeId;
        public int Damage, Range, CritMultiplier, SpellCritMultiplier, Mana,  MulticritCount;
        public float AttackSpeed, TriggerChance, CritChance, ManaRegen;
        public float SpellDamage, SpellCritChance;
  
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio;
        public float BuffDuration, DebuffDuration;
        
        public List<TowerGradeStats> GradeStatList;

        [Expandable]
        public List<Ability> AbilityList;

        public List<float> GradeCostList;
        public List<float> DamageToArmorList;

        private void Awake()
        {
            Level = 1;
        }
    }
}