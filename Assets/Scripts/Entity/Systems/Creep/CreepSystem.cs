using UnityEngine;
using Game.Systems;
using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Creep.System;
using Game.Creep.Data;

namespace Game.Creep
{
    public class CreepSystem : EntitySystem
    {
        public CreepData Stats              { get => stats; set => stats = value; }
        public int WaypointIndex            { get => waypointIndex; set => waypointIndex = value; }
        public EntitySystem LastDamageDealer { get => lastDamageDealer; set => lastDamageDealer = value; }
        public List<AbilitySystem> AbilitySystems { get => abilitySystems; set => abilitySystems = value; }
        public AbilityControlSystem AbilityControlSystem { get => abilityControlSystem; set => abilityControlSystem = value; }
        public List<ICreepTraitSystem> TraitSystems { get => traitSystems; set => traitSystems = value; }
        public TraitControlSystem TraitControlSystem { get => traitControlSystem; set => traitControlSystem = value; }

        private CreepData stats;
        private int waypointIndex;
        private EntitySystem lastDamageDealer;
        private List<AbilitySystem> abilitySystems;
        private List<ICreepTraitSystem> traitSystems;
        private AbilityControlSystem abilityControlSystem;
        private TraitControlSystem traitControlSystem;

        public CreepSystem(GameObject ownerPrefab)
        {
            abilitySystems = new List<AbilitySystem>();   
            traitSystems = new List<ICreepTraitSystem>();  
 
            prefab = ownerPrefab;
        }

        public void SetSystem()
        {
            abilityControlSystem    = new AbilityControlSystem(this);    
            traitControlSystem      = new TraitControlSystem(this);            
            appliedEffectSystem     = new AppliedEffectSystem();
            healthSystem            = new HealthSystem(this);
        
            abilityControlSystem.Set();
        }
    }
}
