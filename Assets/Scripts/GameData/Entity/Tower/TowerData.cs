using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Game.Tower.Data.Stats;
using Game.System;
using Game.Data;

namespace Game.Tower.Data
{  
    [CreateAssetMenu(fileName = "New Tower", menuName = "Data/Tower/Tower")]
    [Serializable]   
    public class TowerData : Entity
    {
        [HideInInspector]
        public int Exp, Level, GradeCount;     

        [HideInInspector]
        public List<float> DamageToRace;

        [ShowAssetPreview(125, 125)]
        public GameObject Prefab;

        [ShowAssetPreview(125, 125)]
        public Sprite Image;

        [BoxGroup("Main Info"), OnValueChanged("OnValuesChanged")]
        public int WaveLevel, ElementLevel, TowerLimit, MagicCrystalReq, GoldCost;
     
        [BoxGroup("Main Info"), OnValueChanged("OnValuesChanged")]
        [SerializeField]
        public RarityType Rarity;

        [BoxGroup("Main Info"), OnValueChanged("OnValuesChanged")]
        [SerializeField]
        public ElementType Element;

        [BoxGroup("Combat Info")]
        public Damage Damage;

        [BoxGroup("Combat Info")]
        public float Range, Mana, ManaRegen, AttackSpeed, TriggerChance, CritChance, CritMultiplier, MulticritCount, SpellDamage, SpellCritChance;       

        [BoxGroup("Ratio")]
        public float GoldRatio, ExpRatio, ItemDropRatio, ItemQuialityRatio, BuffDuration, DebuffDuration;

        [BoxGroup("Special"), Expandable]
        public Special[] SpecialList;     

        [Space, Expandable]
        public List<TowerData> GradeList;

        [Space, Expandable]
        public List<Ability> AbilityList;
        
        protected override void Awake()
        {
           base.Awake();
            
            if(DamageToRace == null)
                for (int i = 0; i < 5; i++)
                    DamageToRace.Add(100f);           
        }

        private void OnValuesChanged() => Id = SetId();

        protected override int[] SetId() => new int[]
                                            {
                                                (int)Element,
                                                (int)Rarity,
                                                (int)WaveLevel,
                                                TowerLimit,
                                                ElementLevel,
                                                MagicCrystalReq,
                                                GoldCost
                                            };
        
    }
}