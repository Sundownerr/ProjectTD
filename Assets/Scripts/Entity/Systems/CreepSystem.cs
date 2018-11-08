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
        private Transform creepTransform;
        private int waypointIndex;
        private StateMachine state;
        private EntitySystem lastDamageDealer;

        protected override void Awake()
        {       
            base.Awake();

            effectSystem = new EffectSystem();
            creepTransform = transform;
            creepTransform.position = GM.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);

            CreepRenderer = transform.GetChild(0).GetComponent<Renderer>();
    
            GM.Instance.CreepList.Add(gameObject);
            GM.Instance.CreepSystemList.Add(this);
            IsVulnerable = true;
            IsOn = true;
        }
    }
}
