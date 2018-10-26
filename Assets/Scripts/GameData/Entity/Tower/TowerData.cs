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

        [ShowAssetPreview(125, 125)]
        public Sprite Image;

        [BoxGroup("Main Info")]
        public int WaveLevel, ElementLevel, TowerLimit, MagicCrystalReq, GoldCost;
     
        [BoxGroup("IDs")]
        public int RarityId, ElementId, DamageTypeId;

        [BoxGroup("Combat Info")]
        public float Damage, Range, Mana, ManaRegen, AttackSpeed, TriggerChance, CritChance, CritMultiplier, MulticritCount, SpellDamage, SpellCritChance;

        [BoxGroup("Ratio")]
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio, BuffDuration, DebuffDuration;

        [BoxGroup("Special")]
        public int MultishotCount, ChainshotCount, AOEShotRange;
        
        [Space]
        public List<TowerData> GradeList;

        [Expandable]
        public List<Ability> AbilityList;
    
        private void Awake()
        {
            Level = 1;
        }        
    }
}