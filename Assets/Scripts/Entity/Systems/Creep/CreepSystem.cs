using UnityEngine;
using Game.Systems;
using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Creep.System;

namespace Game.Creep
{
    public class CreepSystem : EntitySystem
    {
        public CreepData Stats              { get => stats; set => stats = value; }
        public int WaypointIndex            { get => waypointIndex; set => waypointIndex = value; }
        public EntitySystem LastDamageDealer { get => lastDamageDealer; set => lastDamageDealer = value; }
        public List<AbilitySystem> AbilitySystemList { get => abilitySystemList; set => abilitySystemList = value; }
        public AbilityControlSystem AbilityControlSystem { get => abilityControlSystem; set => abilityControlSystem = value; }

        private CreepData stats;
        private int waypointIndex;
        private EntitySystem lastDamageDealer;
        private List<AbilitySystem> abilitySystemList;
        private AbilityControlSystem abilityControlSystem;

        public CreepSystem(GameObject ownerPrefab)
        {
            AbilitySystemList = new List<AbilitySystem>();     
 
            prefab = ownerPrefab;
        }

        public void SetSystem()
        {
            if (stats.AbilityList != null)
                for (int i = 0; i < stats.AbilityList.Count; i++)           
                    AbilitySystemList.Add(new AbilitySystem(stats.AbilityList[i], this));
        
            AbilityControlSystem    = new AbilityControlSystem(this);                
            appliedEffectSystem     = new AppliedEffectSystem();
            healthSystem            = new HealthSystem(this);

            AbilityControlSystem.Set();
        }
    }
}
