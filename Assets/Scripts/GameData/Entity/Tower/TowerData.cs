using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Base Tower")]
    [Serializable]   
    public class TowerData : Entity
    {
        [HideInInspector]
        public int Exp, Level;

        [ShowAssetPreview(125, 125)]
        public GameObject Prefab;

        public Sprite Image;

        [BoxGroup("Main Info")]
        public int Wave, TowerLimit, MagicCrystalRequirement, GoldCost;
     
        [BoxGroup("IDs")]
        public int RarityId, ElementId, DamageTypeId;

        [BoxGroup("Combat Info")]
        public float Damage, Range, Mana, ManaRegen, AttackSpeed, TriggerChance, CritChance, CritMultiplier, MulticritCount, SpellDamage, SpellCritChance;

        [BoxGroup("Ratio")]
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio, BuffDuration, DebuffDuration;

        [BoxGroup("Special")]
        public int MultishotCount, ChainshotCount, AOEShotRange;
        
        [Space]
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