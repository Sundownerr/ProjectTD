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

        [BoxGroup("Special"), Expandable]
        public Special[] SpecialList;     

        [Space, Expandable]
        public List<TowerData> GradeList;

        [Space, Expandable]
        public List<Ability> AbilityList;

        private int level, exp, gradeCount, numberInList;
        [SerializeField]
        private bool isInstanced;

        public void SetData()
        {           
            GradeList = new List<TowerData>();                 
            isInstanced = true;           
            GradeCount = -1;
            
            if (DamageToRace == null)
                for (int i = 0; i < 5; i++)
                    DamageToRace.Add(100f);    

            for (int i = 0; i < GradeList.Count; i++)                
                GradeList[i].Destroy();    
            GradeList.Clear();                 
        }

        [Button("Add to DataBase")]
        private void AddToDataBase()
        {
            if (!IsGradeTower && !IsInstanced)
            {           
                var dataBase = DataLoadingSystem.Load<TowerDataBase>() as TowerDataBase;                       
                var elementList = dataBase.AllTowerList.ElementsList;

                for (int i = 0; i < elementList.Count; i++)
                    if ((int)Element == i)                    
                        for (int j = 0; j < elementList[i].RarityList.Count; j++)
                            if ((int)Rarity == j)
                            {
                                var towerList = elementList[i].RarityList[j].TowerList;
                                for (int k = 0; k < towerList.Count; k++)
                                    if(CompareId(towerList[k].Id))
                                        return;
                                
                                numberInList = towerList.Count;      
                                SetId();  
                                elementList[i].RarityList[j].TowerList.Add(this);                                                           
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
                    dataBase.AllTowerList.ElementsList[(int)Element].RarityList[(int)Rarity].TowerList.RemoveAt(numberInList);  
                    DataLoadingSystem.Save<TowerDataBase>(dataBase);
                }                   
        }

        private void OnDestroy() => RemoveFromDataBase();

        private void OnValuesChanged() => SetId();

        public override void SetId() 
        {
            id = new List<int>
            {
                (int)Element,
                (int)Rarity,
                numberInList,                                                   
            };      
        }
        public void Destroy()
        {
            if (isInstanced)
            {
               
                Destroy(this);                  
            }
        }
    }
}