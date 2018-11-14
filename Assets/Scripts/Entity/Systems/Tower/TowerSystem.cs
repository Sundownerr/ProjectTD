using System;
using System.Collections.Generic;
using Game.Systems;
using Game.Tower.Data;
using Game.Tower.System;
using UnityEngine;

namespace Game.Tower
{
    public class TowerSystem : EntitySystem
    {
        public Transform RangeTransform         { get => rangeTransform; set => rangeTransform = value; }
        public Transform MovingPartTransform    { get => movingPartTransform; set => movingPartTransform = value; }
        public Transform StaticPartTransform    { get => staticPartTransform; set => staticPartTransform = value; }
        public Transform ShootPointTransform    { get => shootPointTransform; set => shootPointTransform = value; }
        public GameObject OcuppiedCell          { get => ocuppiedCell; set => ocuppiedCell = value; }
        public GameObject Bullet            { get => bullet; set => bullet = value; }
        public GameObject Range             { get => range; set => range = value; }
        public Range RangeSystem            {  get => rangeSystem; private set => rangeSystem = value; }
        public Special SpecialSystem        { get => specialSystem; set => specialSystem = value; }
        public Combat CombatSystem          { get => combatSystem; private set => combatSystem = value; }
        public AbilitySystem AbilitySystem  { get => abilitySystem; private set => abilitySystem = value; }
        public Stats StatsSystem            { get => statsSystem;  private set => statsSystem = value; }
        public TowerData Stats              { get => StatsSystem.CurrentStats; set => StatsSystem.CurrentStats = value; }
       

        private Transform rangeTransform, movingPartTransform, staticPartTransform, shootPointTransform;
        private GameObject ocuppiedCell, bullet, target, range;
        private Renderer[] rendererList;
        private Range rangeSystem;
        private Special specialSystem;
        private Combat combatSystem;
        private AbilitySystem abilitySystem;
        private Stats statsSystem;
        private StateMachine state;
        private bool isTowerPlaced;

        protected override void Awake()
        {
            base.Awake();

            movingPartTransform = transform.GetChild(0);
            staticPartTransform = transform.GetChild(1);
            shootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);
            bullet = transform.GetChild(2).gameObject;

            statsSystem     = new Stats(this);
            specialSystem   = new Special(this);
            combatSystem    = new Combat(this);
            abilitySystem   = new AbilitySystem(this);
            effectSystem    = new EffectSystem();

            isVulnerable = false;
            state = new StateMachine();
            state.ChangeState(new SpawnState(this));
        }

        public void SetSystem()
        {
            statsSystem.Set();
            specialSystem.Set();
            combatSystem.Set();
            abilitySystem.Set();

            range = Instantiate(GM.I.RangePrefab, transform);           
            range.transform.localScale = new Vector3(Stats.Range, 0.001f, Stats.Range);
            rangeSystem = range.GetComponent<System.Range>();
            rangeSystem.SetShow();

            rendererList = GetComponentsInChildren<Renderer>();

            bullet.SetActive(false);          
        }

        private void Update()
        {
            if (isOn)
            {
                state.Update();

                if (isTowerPlaced)
                    abilitySystem.Update();
            }

            rangeSystem.SetShow();
        }

        private void SetTowerColor(Color color)
        {
            for (int i = 0; i < rendererList.Length; i++)
                rendererList[i].material.color = color;
        }

        public List<Creep.CreepSystem> GetCreepInRangeList() => RangeSystem.CreepSystemList;

        public void AddExp(int amount) => StatsSystem.AddExp(amount);

        protected class SpawnState : IState
        {
            private readonly TowerSystem o;

            public SpawnState(TowerSystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                void StartPlacing()
                {
                    o.SetTowerColor(GM.I.TowerPlaceSystem.GhostedTowerColor);
                    o.transform.position = GM.I.TowerPlaceSystem.GhostedTowerPos;
                }

                if (GM.PlayerState == State.PlacingTower)
                    StartPlacing();
                else
                    o.state.ChangeState(new LookForCreepState(o));
            }

            public void Exit() 
            {
                void EndPlacing()
                {
                    o.transform.position = o.ocuppiedCell.transform.position;

                    o.SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));

                    var placeEffect = Instantiate(GM.I.ElementPlaceEffectList[(int)o.Stats.Element],
                        o.transform.position + Vector3.up * 5,
                        Quaternion.identity);

                    Destroy(placeEffect, placeEffect.GetComponent<ParticleSystem>().main.duration);

                    o.gameObject.layer = 14;
                    o.rangeSystem.SetShow(false);

                    GM.I.PlayerInputSystem.NewTowerData = null;
                    o.isTowerPlaced = true;
                }

                EndPlacing();  
            }          
        }

        protected class LookForCreepState : IState
        {
            private readonly TowerSystem o;

            public LookForCreepState(TowerSystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                if (o.rangeSystem.CreepList.Count > 0)
                    o.state.ChangeState(new CombatState(o));
            }

            public void Exit() { }
        }

        protected class CombatState : IState
        {
            private readonly TowerSystem o;

            public CombatState(TowerSystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {              
                o.combatSystem.State.Update();

                for (int i = 0; i < o.GetCreepInRangeList().Count; i++)
                    if (o.GetCreepInRangeList()[i] == null)
                    {
                        o.rangeSystem.CreepList.RemoveAt(i);
                        o.rangeSystem.CreepSystemList.RemoveAt(i);
                    }

                if (o.GetCreepInRangeList().Count < 1)
                    o.state.ChangeState(new MoveRemainingBulletState(o));
                else
                    if (o.GetCreepInRangeList()[0] != null)
                    {
                        o.target = o.GetCreepInRangeList()[0].gameObject;
                        RotateAtCreep();
                    }

                void RotateAtCreep()
                {
                    var offset = o.target.transform.position - o.transform.position;
                    offset.y = 0;
                    o.movingPartTransform.rotation = Quaternion.Lerp(o.movingPartTransform.rotation, 
                                                                    Quaternion.LookRotation(offset), 
                                                                    Time.deltaTime * 9f);
                }
            }

            public void Exit() { }
        }

        protected class MoveRemainingBulletState : IState
        {
            private readonly TowerSystem o;

            public MoveRemainingBulletState(TowerSystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                if (o.rangeSystem.CreepList.Count > 0)
                    o.state.ChangeState(new CombatState(o));
                else
                if (!o.combatSystem.CheckAllBulletInactive())
                    o.combatSystem.MoveBullet();
                else
                    o.state.ChangeState(new LookForCreepState(o));
            }

            public void Exit() { }
        }
    }
}