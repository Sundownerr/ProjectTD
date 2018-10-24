using System.Collections;
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
            {
                CachedTransform = transform;
            }

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

        public void GetDamage(float damage, Tower.TowerBaseSystem damageDealer)
        {
            if (!isKilled)
            {
                lastDamageDealer = damageDealer;
                Stats.Health -= damage;

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
            {
                stunDuration = duration;
            }
          
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
            }

            state.ChangeState(new StunnedState(this));
            stunCoroutine = StartCoroutine(RemoveStunnedState(stunDuration));
        }

        private IEnumerator RemoveStunnedState(float delay)
        {
            yield return new WaitForSeconds(delay);

            if(isStunned)
            {
                isStunned = false;
            }

            stunCoroutine = null;
        }

        protected class WalkState : IState
        {
            private CreepSystem owner;

            public WalkState(CreepSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {

            }

            public void Execute()
            {
                owner.waypointReached = owner.CalcDistance(
                        owner.creepTransform.position, 
                        GM.Instance.WaypointList[owner.waypointIndex].transform.position) < (70 + Random.Range(-10, 10));

                if (owner.waypointIndex < GM.Instance.WaypointList.Length - 1)
                {
                    if (!owner.waypointReached)
                    {
                        owner.MoveAndRotateCreep();                       
                    }
                    else
                    {
                        owner.waypointIndex++;
                    }
                }
                else
                {
                    owner.state.ChangeState(new DestroyState(owner));
                }
            }

            public void Exit()
            {

            }
        }

        protected class StunnedState : IState
        {
            private CreepSystem owner;

            public StunnedState(CreepSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.isStunned = true;               
            }

            public void Execute()
            {                         
                if(!owner.isStunned)
                {
                    owner.state.ChangeState(new WalkState(owner));
                }
            }

            public void Exit()
            {

            }
        }

        protected class GiveRecourcesState : IState
        {
            private CreepSystem owner;

            public GiveRecourcesState(CreepSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                owner.lastDamageDealer.AddExp(owner.Stats.Exp);
                GM.Instance.ResourceSystem.AddGold(owner.Stats.Gold);

                owner.state.ChangeState(new DestroyState(owner));
            }

            public void Execute()
            {

            }

            public void Exit()
            {

            }
        }

        protected class DestroyState : IState
        {
            private CreepSystem owner;

            public DestroyState(CreepSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {             
                Destroy(owner.Stats);
                GM.Instance.CreepList.Remove(owner.gameObject);
                Destroy(owner.gameObject);
            }

            public void Execute()
            {

            }

            public void Exit()
            {

            }
        }

    }   
}
