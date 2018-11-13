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
        public Renderer CreepRenderer       { get => creepRenderer; set => creepRenderer = value; }
        public CreepData Stats              { get => stats; set => stats = value; }
        public int WaypointIndex            { get => waypointIndex; set => waypointIndex = value; }
        public EntitySystem LastDamageDealer { get => lastDamageDealer; set => lastDamageDealer = value; }
        public bool IsKilled                { get => isKilled; set => isKilled = value; }

        private bool isKilled;
        private Renderer creepRenderer;
        private CreepData stats;
        private int waypointIndex;
        private EntitySystem lastDamageDealer;

        protected override void Awake()
        {       
            base.Awake();

            effectSystem = new EffectSystem();         
            CreepRenderer = transform.GetChild(0).GetComponent<Renderer>();
    
            GM.I.CreepList.Add(gameObject);
            GM.I.CreepSystemList.Add(this);
            IsVulnerable = true;
            
        }

        private void Start()
        {
            healthSystem = new HealthSystem(this);
        }
    }
}
