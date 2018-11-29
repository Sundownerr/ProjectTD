using UnityEngine;
using Game.Systems;
using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using Game.Data;

namespace Game.Creep
{
    public class CreepSystem : EntitySystem
    {
        public CreepData Stats              { get => stats; set => stats = value; }
        public int WaypointIndex            { get => waypointIndex; set => waypointIndex = value; }
        public EntitySystem LastDamageDealer { get => lastDamageDealer; set => lastDamageDealer = value; }
        
        private CreepData stats;
        private int waypointIndex;
        private EntitySystem lastDamageDealer;
        private List<AbilitySystem> abilitySystemList;

        public CreepSystem(GameObject ownerPrefab)
        {
            abilitySystemList = new List<AbilitySystem>();         
            prefab = ownerPrefab;
        }

        public void SetSystem()
        {
            if(stats.AbilityList != null)
                for (int i = 0; i < stats.AbilityList.Count; i++)           
                    abilitySystemList.Add(new AbilitySystem(stats.AbilityList[i], this));

            healthSystem = new HealthSystem(this);
            appliedEffectSystem = new AppliedEffectSystem();
        }
    }
}
