﻿using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Game.Tower.Data.Stats;
using Game.Data;

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
        private bool isInstanced;
       
        protected override void Awake()
        {                  
            base.Awake();        

            AddToDataBase();       
            
            if(DamageToRace == null)
                for (int i = 0; i < 5; i++)
                    DamageToRace.Add(100f);    

            if(owner == null)
                owner = Prefab == null ? null : Prefab.GetComponent<Tower.TowerSystem>();               
        }

        [Button("Add to DataBase")]
        private void AddToDataBase()
        {
            if(!IsGradeTower && !IsInstanced)
            {
                var database = Resources.Load("TowerDataBase");
                if(database is TowerDataBase dataBase)     
                {            
                    var elementList = dataBase.AllTowerList.ElementsList;
                    for (int i = 0; i < elementList.Count; i++)
                        if(i == (int)Element)                    
                            for (int j = 0; j < elementList[i].RarityList.Count; j++)
                                if(j == (int)Rarity)
                                {
                                    var towerList = elementList[i].RarityList[j].TowerList;
                                    for (int k = 0; k < towerList.Count; k++)
                                        if(CompareId(towerList[k].Id))
                                            if(Name == towerList[k].Name)
                                                return;
                                    
                                    elementList[i].RarityList[j].TowerList.Add(this);
                                    numberInList = towerList.Count - 1;
                                    SetId();
                                    UnityEditor.EditorUtility.SetDirty(dataBase);
                                }     
                }  
            }           
        }

        private void RemoveFromDataBase()
        {
            if(!IsGradeTower && !IsInstanced)
            {
                var database = Resources.Load("TowerDataBase");
                if(database is TowerDataBase dataBase)                            
                   dataBase.AllTowerList.ElementsList[(int)Element].RarityList[(int)Rarity].TowerList.Remove(this);              
            }          
        }

        private void OnDestroy() => RemoveFromDataBase();

        private void OnValuesChanged() => SetId();

        protected override void SetId() => Id = new List<int>
                                                {
                                                    (int)Element,
                                                    (int)Rarity,
                                                    numberInList,                                                   
                                                };            
    }
}