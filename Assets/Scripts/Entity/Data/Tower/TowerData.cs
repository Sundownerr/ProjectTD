using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Game.Tower.Data.Stats;
using Game.Data;
using UnityEditor;
using Game.Systems;
using Newtonsoft.Json;

namespace Game.Tower.Data
{
    [CreateAssetMenu(fileName = "New Tower", menuName = "Data/Tower/Tower")]
    [Serializable]   
    public class TowerData : Entity
    {
        public int Exp { get => exp; set => exp = value; }
        public int Level { get => level; set => level = value > 25 ? 25 : value < 0 ? 0 : value; }
        public int GradeCount { get => gradeCount; set => gradeCount = value; }
        public bool IsInstanced { get => isInstanced; set => isInstanced = value; }
        public int AttackSpeedModifier { get => attackSpeedModifier; set => attackSpeedModifier = value; }

        [HideInInspector]
        public List<float> DamageToRace;     

        [ShowAssetPreview(125, 125)]
        public GameObject Prefab;

        [ShowAssetPreview(125, 125)]
        public Sprite Image;

        [BoxGroup("Main Info")]
        public int WaveLevel, ElementLevel, TowerLimit, MagicCrystalReq, GoldCost;

        [BoxGroup("Main Info")]
        public bool IsGradeTower;
     
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

        [BoxGroup("Trait"), Expandable]
        public List<Trait> Traits;     

        [Space, Expandable]
        public List<TowerData> Grades;

        [Space, Expandable]
        public List<Ability> Abilities;

        private int level, exp, gradeCount, numberInList, attackSpeedModifier;
        [SerializeField]
        private bool isInstanced;

        public void SetData()
        {                   
            Grades = new List<TowerData>();             
            isInstanced = true;           
            GradeCount = -1;
            attackSpeedModifier = 100;
            
            if (DamageToRace == null)
                for (int i = 0; i < 5; i++)
                    DamageToRace.Add(100f);                  
        }

#if UNITY_EDITOR
        [Button("Add to DataBase")]
        private void AddToDataBase()
        {
            if (!IsGradeTower && !IsInstanced)
            {           
                var dataBase = DataLoadingSystem.Load<TowerDataBase>() as TowerDataBase;                       
                var elements = dataBase.AllTowers.Elements;

                for (int i = 0; i < elements.Count; i++)
                    if ((int)Element == i)                    
                        for (int j = 0; j < elements[i].Rarities.Count; j++)
                            if ((int)Rarity == j)
                            {
                                var towers = elements[i].Rarities[j].Towers;
                                for (int k = 0; k < towers.Count; k++)
                                    if (CompareId(towers[k].Id))
                                        return;
                                
                                numberInList = towers.Count;      
                                SetId();  
                                elements[i].Rarities[j].Towers.Add(this);                                                           
                                DataLoadingSystem.Save<TowerDataBase>(dataBase);
                                return;
                            }                    
            }           
        }

        private void RemoveFromDataBase()
        {
            if (!IsGradeTower && !IsInstanced)          
                if (DataLoadingSystem.Load<TowerDataBase>() is TowerDataBase dataBase)            
                {                
                    dataBase.AllTowers.Elements[(int)Element].Rarities[(int)Rarity].Towers.RemoveAt(numberInList);  
                    DataLoadingSystem.Save<TowerDataBase>(dataBase);
                }                   
        }

        private void OnDestroy() => RemoveFromDataBase();

        private void OnValuesChanged() => SetId();
#endif
        public override void SetId() 
        {
            id = new List<int>
            {
                (int)Element,
                (int)Rarity,
                numberInList,                                                   
            };      
        }     
    }
}