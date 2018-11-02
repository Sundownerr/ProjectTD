using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Tower.Data;

namespace Game.Tower
{
    public class TowerSystem : EntitySystem
    {
        [HideInInspector]
        public Transform RangeTransform, MovingPartTransform, StaticPartTransform, ShootPointTransform;

        [HideInInspector]
        public GameObject OcuppiedCell, Bullet, Range;        
  
        private System.Range rangeSystem;       
        private Renderer[] rendererList;
        private System.Special specialSystem;
        private System.Combat combatSystem;
        private System.AbilitySystem abilitySystem;
        private System.Stats statsSystem;
        private StateMachine state;
        private GameObject Target;
        private bool isTowerPlaced;

        protected override void Awake()
        {
            base.Awake();         

            MovingPartTransform = transform.GetChild(0);
            StaticPartTransform = transform.GetChild(1);
            ShootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);
            Bullet = transform.GetChild(2).gameObject;

            statsSystem     = new System.Stats(this);
            specialSystem   = new System.Special(this);
            combatSystem    = new System.Combat(this);
            abilitySystem   = new System.AbilitySystem(this);

            IsVulnerable = false;
            state = new StateMachine();
            state.ChangeState(new SpawnState(this));
        }

        public void SetSystem()
        {
            statsSystem.Set();
            specialSystem.Set();
            combatSystem.Set();
            abilitySystem.Set();

            Range                       = Instantiate(GM.Instance.RangePrefab, transform);
            rangeSystem                 = Range.GetComponent<System.Range>();
            Range.transform.localScale  = new Vector3(GetStats().Range, 0.001f, GetStats().Range);
            rangeSystem.SetShow();

            rendererList = GetComponentsInChildren<Renderer>();

            Bullet.SetActive(false);
            statsSystem.UpdateUI();
        }

        private void Update()
        {
            if (isOn)
            {
                state.Update();

                if (isTowerPlaced)
                    abilitySystem.State.Update();
            }
            
            rangeSystem.SetShow();
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

            var placeEffect = Instantiate(GM.Instance.ElementPlaceEffectList[(int)GetStats().Element], transform.position + Vector3.up * 5, Quaternion.identity);
            Destroy(placeEffect, placeEffect.GetComponent<ParticleSystem>().main.duration);

            gameObject.layer = 14;
            rangeSystem.SetShow(false);
          
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

        public TowerData GetStats() => statsSystem.CurrentStats;  

        public List<Creep.CreepSystem> GetCreepInRangeList() => rangeSystem.CreepSystemList;
    
        public System.Special GetSpecial() => specialSystem;     

        public void SetStats(TowerData stats) => statsSystem.CurrentStats = stats;   

        public void AddExp(int amount) => statsSystem.AddExp(amount);
        
        public void Upgrade()
        {
            var isGradeCountOk =
                GetStats().GradeList.Count > 0 &&
                GetStats().GradeCount < GetStats().GradeList.Count;

            if (isGradeCountOk)
            {
                var upgradedTowerPrefab = Instantiate(GetStats().GradeList[0].Prefab, transform.position, Quaternion.identity, GM.Instance.TowerParent);
                var upgradedTowerSystem = upgradedTowerPrefab.GetComponent<TowerSystem>();
               
                upgradedTowerSystem.statsSystem.Upgrade(GetStats(), GetStats().GradeList[0]);
                upgradedTowerSystem.OcuppiedCell = OcuppiedCell;
                upgradedTowerSystem.SetSystem();

                GM.Instance.PlayerInputSystem.ChoosedTower = upgradedTowerPrefab;
                
                Destroy(gameObject);
            }
        }

        public void Sell()
        {
            GM.Instance.ResourceSystem.AddTowerLimit(-GetStats().TowerLimit);
            GM.Instance.ResourceSystem.AddGold(GetStats().GoldCost);

            OcuppiedCell.GetComponent<Cells.Cell>().IsBusy = false;
            GM.Instance.PlacedTowerList.Remove(gameObject);
            Destroy(gameObject);
        }

        protected class SpawnState : IState
        {
            private readonly TowerSystem o;

            public SpawnState(TowerSystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                if (GM.PlayerState == GM.State.PlacingTower)
                    o.StartPlacing();
                else
                    o.state.ChangeState(new LookForCreepState(o));               
            }

            public void Exit()
            {
                o.EndPlacing();
                o.statsSystem.UpdateUI();
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
                    o.Target = o.GetCreepInRangeList()[0].gameObject;         
                              
                if (o.Target != null)
                    o.RotateAtCreep(o.Target);         
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
                if(!o.combatSystem.CheckAllBulletInactive())
                    o.combatSystem.MoveBullet();
                else
                    o.state.ChangeState(new LookForCreepState(o));
            }         

            public void Exit() { }
        }
    }
}
