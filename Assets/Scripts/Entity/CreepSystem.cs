using UnityEngine;
using Game.Systems;
using Game.Tower;


namespace Game.Creep
{
    public class CreepSystem : EntitySystem
    {
        public bool ReachedLastWaypoint { get => reachedLastWaypoint; set => reachedLastWaypoint = value; }
        public Renderer CreepRenderer { get => creepRenderer; set => creepRenderer = value; }

        private bool reachedLastWaypoint;
        private Renderer creepRenderer;
        private CreepData stats;
        private Transform creepTransform;
        private bool waypointReached;
        private int waypointIndex;
        private StateMachine state;
        private EntitySystem lastDamageDealer;
        private bool isKilled;

        protected override void Awake()
        {
            base.Awake();

            creepTransform = transform;
            creepTransform.position = GM.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);

            CreepRenderer = transform.GetChild(0).GetComponent<Renderer>();
    
            GM.Instance.CreepList.Add(gameObject);
            IsVulnerable = true;
            state = new StateMachine();
            state.ChangeState(new WalkState(this));
        }

        private void Update()
        { 
            if(isOn)
                state.Update();
        }

        public CreepData GetStats() => stats;


        public void SetStats(CreepData stats)
        {
            this.stats = Instantiate(stats);
            this.stats = stats;
        } 

        public EntitySystem GetDamageDealer() => lastDamageDealer;
        
        private void MoveAndRotateCreep()
        {
            creepTransform.Translate(Vector3.forward * Time.deltaTime * GetStats().MoveSpeed, Space.Self);

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

        private float CalculateDamage(float rawDamage, EntitySystem damageDealer)
        {
            var damage = 0f;

            if(damageDealer is Tower.TowerSystem tower)       
                damage = GetPercentOfValue(tower.GetStats().DamageToRace[(int)stats.Race], rawDamage);

            // add armor modificator

            return damage;
        }

        public void GetDamage(float damage, EntitySystem damageDealer)
        {
            if (!isKilled)
            {
                lastDamageDealer = damageDealer;
                stats.Health -= CalculateDamage(damage, lastDamageDealer);

                if (stats.Health <= 0)
                {
                    isKilled = true;
                    state.ChangeState(new GiveRecourcesState(this));
                }
            }
        }

        protected class WalkState : IState
        {
            private readonly CreepSystem o;

            public WalkState(CreepSystem o) => this.o = o;

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

        protected class GiveRecourcesState : IState
        {
            private readonly CreepSystem o;

            public GiveRecourcesState(CreepSystem o) => this.o = o;

            public void Enter()
            {
                var tower = o.lastDamageDealer as TowerSystem;

                if (tower != null)
                {
                    tower.AddExp(o.stats.Exp);
                    GM.Instance.ResourceSystem.AddGold(o.stats.Gold);
                }
              
                o.state.ChangeState(new DestroyState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class DestroyState : IState
        {
            private readonly CreepSystem o;

            public DestroyState(CreepSystem o) => this.o = o;

            public void Enter()
            {             
                GM.Instance.CreepList.Remove(o.gameObject);
                Destroy(o.gameObject);
            }

            public void Execute() { }

            public void Exit() { }
        }
    }   
}
