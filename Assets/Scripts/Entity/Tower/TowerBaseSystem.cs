using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;

namespace Game.Tower
{
    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public Transform RangeTransform, MovingPartTransform, StaticPartTransform, ShootPointTransform;

        [HideInInspector]
        public GameObject OcuppiedCell, Bullet, Range;        

        [HideInInspector]
        public TowerRangeSystem RangeSystem;

        [HideInInspector]
        public TowerStatsSystem StatsSystem;

        [HideInInspector]
        public TowerSpecialSystem specialSystem;

        private Renderer[] rendererList;
        private TowerCombatSystem combatSystem;
        private TowerAbilitySystem abilitySystem;
        private StateMachine state;
        private GameObject Target;
        private bool isTowerPlaced;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
                CachedTransform = transform;            

            MovingPartTransform = transform.GetChild(0);
            StaticPartTransform = transform.GetChild(1);
            ShootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);
            Bullet = transform.GetChild(2).gameObject;

            StatsSystem = new TowerStatsSystem(this);
            specialSystem = new TowerSpecialSystem(this);
            combatSystem = new TowerCombatSystem(this);
            abilitySystem = new TowerAbilitySystem(this);

            state = new StateMachine();
            state.ChangeState(new SpawnState(this));
        }

        public void SetSystem()
        {
            StatsSystem.Set();
            specialSystem.Set();
            combatSystem.Set();
            abilitySystem.Set();

            Range = Instantiate(GM.Instance.RangePrefab, transform);
            RangeSystem = Range.GetComponent<TowerRangeSystem>();
            Range.transform.localScale = new Vector3(StatsSystem.Stats.Range, 0.001f, StatsSystem.Stats.Range);
            RangeSystem.SetShow();

            rendererList = GetComponentsInChildren<Renderer>();

            Bullet.SetActive(false);
            StatsSystem.UpdateUI();
        }

        private void Update()
        {
            state.Update();

            if (isTowerPlaced)
                abilitySystem.State.Update();
            
            RangeSystem.SetShow();
        }
       
        private void SetTowerColor(Color color)
        {
            for (int i = 0; i < rendererList.Length; i++)
                rendererList[i].material.color = color;
        }

        private void StartPlacing()
        {
            SetTowerColor(GM.Instance.TowerPlaceSystem.GhostedTowerColor);
            transform.position = GM.Instance.TowerPlaceSystem.GhostedTowerPos;
        }

        private void EndPlacing()
        {          
            transform.position = OcuppiedCell.transform.position;
            
            SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));

            var placeEffect = Instantiate(GM.Instance.ElementPlaceEffectList[StatsSystem.Stats.ElementId], transform.position + Vector3.up * 5, Quaternion.identity);
            Destroy(placeEffect, placeEffect.GetComponent<ParticleSystem>().main.duration);

            gameObject.layer = 14;
            RangeSystem.SetShow(false);
          
            GM.Instance.PlayerInputSystem.NewTowerData = null;           

            isTowerPlaced = true;
        }

        private void RotateAtCreep(GameObject target)
        {
            var offset = target.transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            MovingPartTransform.rotation = Quaternion.Lerp(MovingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }

        public void Upgrade()
        {
            var isGradeCountOk =
                StatsSystem.Stats.GradeList.Count > 0 &&
                StatsSystem.Stats.GradeCount < StatsSystem.Stats.GradeList.Count;

            if (isGradeCountOk)
            {
                var upgradedTowerPrefab = Instantiate(StatsSystem.Stats.GradeList[0].Prefab, transform.position, Quaternion.identity, GM.Instance.TowerParent);
                var upgradedTowerBaseSystem = upgradedTowerPrefab.GetComponent<TowerBaseSystem>();
               
                upgradedTowerBaseSystem.StatsSystem.Upgrade(StatsSystem.Stats, StatsSystem.Stats.GradeList[0]);
                upgradedTowerBaseSystem.OcuppiedCell = OcuppiedCell;
                upgradedTowerBaseSystem.SetSystem();

                GM.Instance.PlayerInputSystem.ChoosedTower = upgradedTowerPrefab;
                
                Destroy(gameObject);
            }
        }

        public void Sell()
        {
            GM.Instance.ResourceSystem.AddTowerLimit(-StatsSystem.Stats.TowerLimit);
            GM.Instance.ResourceSystem.AddGold(StatsSystem.Stats.GoldCost);

            OcuppiedCell.GetComponent<Cells.Cell>().IsBusy = false;
            GM.Instance.PlacedTowerList.Remove(gameObject);
            Destroy(gameObject);
        }

        protected class SpawnState : IState
        {
            private readonly TowerBaseSystem o;

            public SpawnState(TowerBaseSystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                if (GM.PLAYERSTATE == GM.PLACING_TOWER)
                    o.StartPlacing();
                else
                    o.state.ChangeState(new LookForCreepState(o));               
            }

            public void Exit()
            {
                o.EndPlacing();
                o.StatsSystem.UpdateUI();
            }
        }

        protected class LookForCreepState : IState
        {
            private readonly TowerBaseSystem o;

            public LookForCreepState(TowerBaseSystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                if (o.RangeSystem.CreepList.Count > 0)
                    o.state.ChangeState(new CombatState(o));                  
            }

            public void Exit() { }
        }

        protected class CombatState : IState
        {
            private readonly TowerBaseSystem o;

            public CombatState(TowerBaseSystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                o.combatSystem.State.Update();

                for (int i = 0; i < o.RangeSystem.CreepList.Count; i++)
                    if (o.RangeSystem.CreepList[i] == null)
                    {
                        o.RangeSystem.CreepList.RemoveAt(i);
                        o.RangeSystem.CreepSystemList.RemoveAt(i);
                    }

                if (o.RangeSystem.CreepList.Count < 1)
                    o.state.ChangeState(new MoveRemainingBulletState(o));
                else
                    o.Target = o.RangeSystem.CreepList[0];         
                              
                if (o.Target != null)
                    o.RotateAtCreep(o.Target);         
            }

            public void Exit() { }
        }

        protected class MoveRemainingBulletState : IState
        {
            private readonly TowerBaseSystem o;

            public MoveRemainingBulletState(TowerBaseSystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                if (o.RangeSystem.CreepList.Count > 0)
                    o.state.ChangeState(new CombatState(o));
                else 
                if(!o.combatSystem.CheckAllBulletInactive())
                    o.combatSystem.MoveBullet();
                else
                    o.state.ChangeState(new LookForCreepState(o));
            }         

            public void Exit() { }
        }
    }
}
