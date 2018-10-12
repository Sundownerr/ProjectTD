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

        public CreepStats Stats;

        [HideInInspector]
        public Renderer creepRenderer;

        private Transform creepTransform;
        private bool waypointReached, isStunned;
        private int waypointIndex;
        private StateMachine state;
        private Coroutine stunCoroutine;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            creepTransform = transform;
            creepTransform.position = GameManager.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);

            creepRenderer = transform.GetChild(0).GetComponent<Renderer>();

            Stats = Instantiate(Stats);

            state = new StateMachine();
            state.ChangeState(new WalkState(this));

            GameManager.Instance.CreepList.Add(gameObject);
            transform.parent = GameManager.Instance.CreepParent;
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
            var lookRotation = Quaternion.LookRotation(GameManager.Instance.WaypointList[waypointIndex].transform.position - creepTransform.position);
            var rotation = Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);
            rotation.z = 0;
            rotation.x = 0;

            creepTransform.localRotation = rotation;
        }

        public void GetDamage(int damage)
        {
            Stats.Health -= damage;

            if (Stats.Health <= 0)
            {
                state.ChangeState(new DestroyState(this));
            }         
        }

        public void GetStunned(float duration)
        {
            state.ChangeState(new StunnedState(this, duration));

            if (stunCoroutine != null)
            {
                StopCoroutine(RemoveStunnedState(duration));
            }

            stunCoroutine = StartCoroutine(RemoveStunnedState(duration));
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

        public class WalkState : IState
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
                owner.waypointReached = GameManager.CalcDistance(
                        owner.creepTransform.position, 
                        GameManager.Instance.WaypointList[owner.waypointIndex].transform.position) < (70 + Random.Range(-10, 10));

                if (owner.waypointIndex < GameManager.Instance.WaypointList.Length - 1)
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

        public class StunnedState : IState
        {
            private CreepSystem owner;
            private float duration;

            public StunnedState(CreepSystem owner, float duration)
            {
                this.owner = owner;
                this.duration = duration;
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
    }

    public class DestroyState : IState
    {
        private CreepSystem owner;

        public DestroyState(CreepSystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            Object.Destroy(owner.Stats);
            GameManager.Instance.CreepList.Remove(owner.gameObject);
            Object.Destroy(owner.gameObject);
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}
