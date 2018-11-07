using UnityEngine;
using Game.Creep.Data;
using System.Collections.Generic;
using Game.Data;
using NaughtyAttributes;
using Game.Systems;

namespace Game.Creep
{  
    public class CreepData : Entity
    {

        public float MoveSpeed  { get => moveSpeed; set => moveSpeed = value >= 0 ? value : 0; }
        public int Exp          { get => exp; set => exp = value; }
        public int Gold         { get => gold; set => gold = value >= 0 ? value : 0; }      
        public float Health     { get => health; set => health = value >= 0 ? value : 0; }
        public float DefaultMoveSpeed       { get => defaultMoveSpeed; set => defaultMoveSpeed = value; }
        public CreepType Type   { get => type; set => type = value; }
        public List<Ability> AbilityList    { get => abilityList; set => abilityList = value; }
        public float ArmorValue { get => armorValue; set => armorValue = value; }
        public Armor.ArmorType ArmorType    { get => armorType; set => armorType = value; }
        public bool IsInstanced { get => isInstanced; set => isInstanced = value; }

        [ShowAssetPreview(125, 125)]
        public GameObject Prefab;
        public int WaveLevel;
  
        public RaceType Race;

        protected Armor.ArmorType armorType;
        protected float armorValue;
        protected int gold, exp, numberInList;
        protected float defaultMoveSpeed, moveSpeed, health;    
        protected CreepType type;
        protected bool isInstanced;
        protected List<Ability> abilityList;

        protected virtual void Awake() 
        {
            AddToDataBase();                   
            
            if(Owner == null)
                Owner = Prefab == null ? null : Prefab.GetComponent<Tower.TowerSystem>();      
        }

        [Button]
        public void AddToDataBase()
        {
            if(!IsInstanced)
            {
                var database = Resources.Load("CreepDataBase");
                if(database is CreepDataBase dataBase)     
                {            
                    var raceList = dataBase.AllCreepList;
                    for (int i = 0; i < raceList.Count; i++)
                        if(i == (int)Race)                    
                        {
                            for (int j = 0; j < raceList[i].CreepList.Count; j++)                             
                                if(CompareId(raceList[i].CreepList[j].Id) || Name == raceList[i].CreepList[j].Name)
                                    return;
                            
                            raceList[i].CreepList.Add(this);
                            numberInList = raceList[i].CreepList.Count - 1;    

                            SetId();
                            SetName();                        
                            
                            UnityEditor.EditorUtility.SetDirty(dataBase);
                            return;
                        }
                }
            }           
        }

        private void RemoveFromDataBase()
        {
            if(!IsInstanced)
            {
                var database = Resources.Load("CreepDataBase");
                if(database is CreepDataBase dataBase)             
                {               
                   dataBase.AllCreepList[(int)Race].CreepList.RemoveAt(numberInList);       

                   UnityEditor.EditorUtility.SetDirty(dataBase);       
                }    
            }      
        }

        protected override void SetName()
        {
            var tempName = new System.Text.StringBuilder();
        
            tempName.Append(Race.ToString());
            tempName.Append(Type.ToString());
            tempName.Append((int)Race);
            tempName.Append((int)Type);
            tempName.Append(numberInList);

            Name = tempName.ToString();
        }

        private void OnDestroy() => RemoveFromDataBase();

        public void OnValuesChanged() => AddToDataBase();

        public override void SetId() => id = new List<int>
                                                {
                                                    (int)Race,
                                                    (int)Type,
                                                    numberInList
                                                };
    }
}
