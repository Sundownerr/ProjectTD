using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Base Tower")]
    public class TowerData : Entity
    {
        public string Element;
        public int Wave, Exp, Level, TowerLimit, MagicCrystalRequirement, GoldCost, RarityId, DamageTypeId;
        public int Damage, Range, CritMultiplier, SpellCritMultiplier, Mana,  MulticritCount;
        public float AttackSpeed, TriggerChance, CritChance, ManaRegen;
        public float SpellDamage, SpellCritChance;
  
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio;
        public float BuffDuration, DebuffDuration;

        public int MultishotCount, ChainshotCount, AOEShotRange;
        
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