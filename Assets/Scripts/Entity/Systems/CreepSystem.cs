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
    }
}
