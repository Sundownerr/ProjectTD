using UnityEngine;
using Game.Creep.Data;
using System.Collections.Generic;
using Game.Data;
using NaughtyAttributes;

namespace Game.Creep
{  
    public class CreepData : Entity
    {

        public float MoveSpeed  { get => moveSpeed; set => moveSpeed = value >= 0 ? value : 0; }
        public int Exp          { get => exp; set => exp = value; }
        public int Gold         { get => gold; set => gold = value >= 0 ? value : 0; }
        public int WaveLevel    { get => waveLevel; set => waveLevel = value; }
        public float Health     { get => health; set => health = value >= 0 ? value : 0; }
        public float DefaultMoveSpeed       { get => defaultMoveSpeed; set => defaultMoveSpeed = value; }
        public CreepType Type   { get => type; set => type = value; }
        public List<Ability> AbilityList    { get => abilityList; set => abilityList = value; }
        public float ArmorValue { get => armorValue; set => armorValue = value; }
        public Armor.ArmorType ArmorType    { get => armorType; set => armorType = value; }

        [ShowAssetPreview(125, 125)]
        public GameObject Prefab;
        public RaceType Race;

        private Armor.ArmorType armorType;
        private float armorValue;
        private int waveLevel, gold, exp;
        private float defaultMoveSpeed, moveSpeed, health;    
        private CreepType type;
        private List<Ability> abilityList;

        protected new virtual void Awake() 
        {
            base.Awake();

              if(owner == null)
                owner = Prefab == null ? null : Prefab.GetComponent<Tower.TowerSystem>();      
        }

        public void OnValuesChanged() => SetId();

        protected override void SetId() => Id = new List<int>
                                                {
                                                    (int)Race,
                                                    (int)ArmorType,
                                                    (int)Type,
                                                    WaveLevel,
                                                    Exp,
                                                    Gold
                                                };
    }
}
