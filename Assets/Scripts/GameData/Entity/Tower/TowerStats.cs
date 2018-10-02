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
        public GameObject StaticPart, MovingPart, ShootingPart;
        public int Damage, Range, CritMultiplier, SpellCritMultiplier, Mana,  MulticritCount;
        public float AttackSpeed, TriggerChance, CritChance, ManaRegen;
        public float SpellDamage, SpellCritChance;
  
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio;
        public float BuffDuration, DebuffDuration;
        
        public TowerGradeStats[] TowerGradeStats;
        public float[] GradeCosts;
        public float[] DamageToArmor;

    }
}