﻿using System.Collections;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Creep;

namespace Game.Creep
{
    public class CreepSystem : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public bool ReachedLastWaypoint;

        public CreepData Stats;

        [HideInInspector]
        public Renderer creepRenderer;

        private Transform creepTransform;
        private bool waypointReached, isStunned;
        private int waypointIndex;
        private StateMachine state;
        private Coroutine stunCoroutine;
        private float stunDuration;
        private Tower.TowerBaseSystem lastDamageDealer;
        private bool isKilled;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
                CachedTransform = transform;

            creepTransform = transform;
            creepTransform.position = GM.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);

            creepRenderer = transform.GetChild(0).GetComponent<Renderer>();

            Stats = Instantiate(Stats);           
            
            transform.parent = GM.Instance.CreepParent;

            GM.Instance.CreepList.Add(gameObject);

            state = new StateMachine();
            state.ChangeState(new WalkState(this));
        }

        private void Update()
        {
            state.Update();
        }

        private void MoveAndRotateCreep()
        {
            creepTransform.Translate(Vector3.forward * Time.deltaTime * Stats.MoveSpeed, Space.Self);

            var clampPos = new Vector3(creepTransform.position.x, creepTransform.lossyScale.y, creepTransform.position.z);
            creepTransform.position = clampPos;

            RotateCreep();
        }

        private void RotateCreep()
        {
            var lookRotation = Quaternion.LookRotation(GM.Instance.WaypointList[waypointIndex].transform.position - creepTransform.position);
            var rotation = Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);
            rotation.z = 0;
            rotation.x = 0;

            creepTransform.localRotation = rotation;
        }

        private float CalculateDamage(float rawDamage, Tower.TowerBaseSystem damageDealer)
        {
            var damage = GetPercentOfValue(damageDealer.StatsSystem.Stats.DamageToRace[Stats.RaceId], rawDamage);
            // add armor modificator
            return damage;
        }

        public void GetDamage(float damage, Tower.TowerBaseSystem damageDealer)
        {
            if (!isKilled)
            {
                lastDamageDealer = damageDealer;
                Stats.Health -= CalculateDamage(damage, lastDamageDealer);

                if (Stats.Health <= 0)
                {
                    isKilled = true;
                    state.ChangeState(new GiveRecourcesState(this));
                }
            }
        }

        public void GetStunned(float duration)
        {
            if(duration > stunDuration)
                stunDuration = duration;
          
            if (stunCoroutine != null)
                StopCoroutine(stunCoroutine);           

            state.ChangeState(new StunnedState(this));
            stunCoroutine = StartCoroutine(RemoveStunnedState(stunDuration));
        }

        private IEnumerator RemoveStunnedState(float delay)
        {
            yield return new WaitForSeconds(delay);

            if(isStunned)
                isStunned = false;
            
            stunCoroutine = null;
        }

        protected class WalkState : IState
        {
            private CreepSystem o;

            public WalkState(CreepSystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                o.waypointReached = CalcDistance(
                        o.creepTransform.position, 
                        GM.Instance.WaypointList[o.waypointIndex].transform.position) < (70 + Random.Range(-10, 10));

                if (o.waypointIndex < GM.Instance.WaypointList.Length - 1)
                    if (!o.waypointReached)
                        o.MoveAndRotateCreep();                       
                    else
                        o.waypointIndex++;
                else
                    o.state.ChangeState(new DestroyState(o));
            }

            public void Exit() { }
        }

        protected class StunnedState : IState
        {
            private CreepSystem o;

            public StunnedState(CreepSystem o) { this.o = o; }

            public void Enter()
            {
                o.isStunned = true;               
            }

            public void Execute()
            {                         
                if(!o.isStunned)
                    o.state.ChangeState(new WalkState(o));
            }

            public void Exit() { }
        }

        protected class GiveRecourcesState : IState
        {
            private CreepSystem o;

            public GiveRecourcesState(CreepSystem o) { this.o = o; }

            public void Enter()
            {
                o.lastDamageDealer.StatsSystem.AddExp(o.Stats.Exp);
                GM.Instance.ResourceSystem.AddGold(o.Stats.Gold);

                o.state.ChangeState(new DestroyState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class DestroyState : IState
        {
            private CreepSystem o;

            public DestroyState(CreepSystem o) { this.o = o; }

            public void Enter()
            {             
                Destroy(o.Stats);
                GM.Instance.CreepList.Remove(o.gameObject);
                Destroy(o.gameObject);
            }

            public void Execute() { }

            public void Exit() { }
        }
    }   
}
