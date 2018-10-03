using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Base Tower")]
    public class TowerStats : Entity
    {
        private void OnEnable()
        {
            
        }
        public int Id, Exp, Level;
        public GameObject StaticPart, MovingPart, ShootingPart;
        public int Damage, Range, CritMultiplier, SpellCritMultiplier, Mana,  MulticritCount;
        public float AttackSpeed, TriggerChance, CritChance, ManaRegen;
        public float SpellDamage, SpellCritChance;
  
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio;
        public float BuffDuration, DebuffDuration;
        
        public List<TowerGradeStats> TowerGradeStatList;
        public List<Ability> TowerAbilityList;
        public List<float> GradeCostList;
        public List<float> DamageToArmorList;

    }
}