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
        public List<AbilitySystem> AbilitySystems { get => abilitySystems; set => abilitySystems = value; }
        public AbilityControlSystem AbilityControlSystem { get => abilityControlSystem; set => abilityControlSystem = value; }

        private CreepData stats;
        private int waypointIndex;
        private EntitySystem lastDamageDealer;
        private List<AbilitySystem> abilitySystems;
        private AbilityControlSystem abilityControlSystem;

        public CreepSystem(GameObject ownerPrefab)
        {
            AbilitySystems = new List<AbilitySystem>();     
 
            prefab = ownerPrefab;
        }

        public void SetSystem()
        {
            if (stats.Abilities != null)
                for (int i = 0; i < stats.Abilities.Count; i++)           
                    AbilitySystems.Add(new AbilitySystem(stats.Abilities[i], this));
        
            AbilityControlSystem    = new AbilityControlSystem(this);                
            appliedEffectSystem     = new AppliedEffectSystem();
            healthSystem            = new HealthSystem(this);

            AbilityControlSystem.Set();
        }
    }
}
