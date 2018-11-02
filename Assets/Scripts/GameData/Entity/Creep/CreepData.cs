using UnityEngine;
using Game.Creep.Data;
using System.Collections.Generic;
using Game.Data;
using NaughtyAttributes;

namespace Game.Creep
{  
    public class CreepData : Entity
    {
        [HideInInspector]
        public float MoveSpeed;

        [ShowAssetPreview(125, 125)]
        public GameObject Prefab;

        [BoxGroup("Base Info"), OnValueChanged("OnValuesChanged")]
        public int Exp, Gold, WaveLevel;

        [BoxGroup("Combat Info"), OnValueChanged("OnValuesChanged")]
        public float Health, DefaultMoveSpeed;

        [BoxGroup("Combat Info"), OnValueChanged("OnValuesChanged")]
        public Armor Armor;

        [BoxGroup("Combat Info"), OnValueChanged("OnValuesChanged")]
        public RaceType Race;
      
        [Expandable, BoxGroup("Combat Info")]
        public List<Ability> AbilityList;

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
                                                    (int)Armor.Type,
                                                    WaveLevel,
                                                    Exp,
                                                    Gold
                                                };
    }
}
