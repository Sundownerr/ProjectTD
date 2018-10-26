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

        public GameObject TowerPlaceEffect;

        protected List<Renderer> rendererList;
    
        private TowerCombatSystem combatSystem;
        private TowerAbilitySystem abilitySystem;
        private StateMachine state;
        private bool isRangeShowed, isTowerPlaced;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }        

            combatSystem = GetComponent<TowerCombatSystem>();
            abilitySystem = GetComponent<TowerAbilitySystem>();
            StatsSystem = GetComponent<TowerStatsSystem>();        

            MovingPartTransform = transform.GetChild(0);
            StaticPartTransform = transform.GetChild(1);
            ShootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);
            Bullet = transform.GetChild(2).gameObject;
         
            state = new StateMachine();
            state.ChangeState(new SpawnState(this));
        }

        private void Start()
        {
            Range = Instantiate(GM.Instance.RangePrefab, transform);
            RangeSystem = Range.GetComponent<TowerRangeSystem>();
            Range.transform.localScale = new Vector3(StatsSystem.Stats.Range, 0.001f, StatsSystem.Stats.Range);

            rendererList = new List<Renderer>();
            rendererList.AddRange(GetComponentsInChildren<Renderer>());

            isRangeShowed = true;

            StatsSystem.UpdateUI();
        }

        private void Update()
        {
            state.Update();

            if (isTowerPlaced)
            {
                abilitySystem.state.Update();
            }

            SetRangeShow();
        }

        private void SetRangeShow()
        {
            var isChoosedTower = 
                GM.Instance.TowerUISystem.gameObject.activeSelf && 
                GM.Instance.PlayerInputSystem.ChoosedTower == gameObject;

            if (isChoosedTower && !isRangeShowed)
            {
                RangeSystem.Show(true);
                isRangeShowed = true;
            }
            else
            if (!isChoosedTower && isRangeShowed)
            {
                RangeSystem.Show(false);
                isRangeShowed = false;
            }
        }

        private void SetTowerColor(Color color)
        {
            for (int i = 0; i < rendererList.Count; i++)
            {
                rendererList[i].material.color = color;
            }
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

            var placeEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.identity);
            Destroy(placeEffect, placeEffect.GetComponent<ParticleSystem>().main.startLifetime.constant);

            gameObject.layer = 14;
            RangeSystem.Show(false);

          
            GM.Instance.ChoosedTowerData = null;           

            isTowerPlaced = true;
        }

        private void RotateAtCreep()
        {
            var offset = RangeSystem.CreepList[0].transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            MovingPartTransform.rotation = Quaternion.Lerp(MovingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }

        public void Upgrade()
        {
            if (StatsSystem.Stats.GradeList.Count > 0 && StatsSystem.Stats.GradeCount < StatsSystem.Stats.GradeList.Count)
            {
                var upgradedTower = Instantiate(StatsSystem.Stats.GradeList[0].Prefab, transform.position, Quaternion.identity, GM.Instance.TowerParent);
                var upgradedTowerBaseSystem = upgradedTower.GetComponent<TowerBaseSystem>();
              
                upgradedTowerBaseSystem.StatsSystem.Upgrade(upgradedTowerBaseSystem.StatsSystem.BaseStats, StatsSystem.Stats.GradeList[0]);
                upgradedTowerBaseSystem.StatsSystem.Stats.Level = StatsSystem.Stats.Level;
                upgradedTowerBaseSystem.StatsSystem.Stats.Exp = StatsSystem.Stats.Exp;

                upgradedTowerBaseSystem.OcuppiedCell = OcuppiedCell;

                GM.Instance.PlayerInputSystem.ChoosedTower = upgradedTower;
                
                Destroy(gameObject);
            }
        }

        public void Sell()
        {
            GM.Instance.ResourceSystem.AddTowerLimit(-StatsSystem.Stats.TowerLimit);
            GM.Instance.ResourceSystem.AddGold(StatsSystem.Stats.GoldCost);

            OcuppiedCell.GetComponent<TowerCells.Cell>().IsBusy = false;
            GM.Instance.PlacedTowerList.Remove(gameObject);
            Destroy(gameObject);
        }

        protected class SpawnState : IState
        {
            private readonly TowerBaseSystem owner;

            public SpawnState(TowerBaseSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {                                                           
            }

            public void Execute()
            {
                if (GM.PLAYERSTATE == GM.PLACING_TOWER)
                {
                    owner.StartPlacing();
                }
                else
                {
                    owner.state.ChangeState(new LookForCreepState(owner));
                }
            }

            public void Exit()
            {
                owner.EndPlacing();

                owner.StatsSystem.UpdateUI();
            }
        }

        protected class LookForCreepState : IState
        {
            private readonly TowerBaseSystem owner;

            public LookForCreepState(TowerBaseSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
           
            }

            public void Execute()
            {
                if (owner.RangeSystem.CreepList.Count > 0)
                {
                    owner.state.ChangeState(new CombatState(owner));                  
                }
            }

            public void Exit()
            {
            }
        }

        protected class CombatState : IState
        {
            private readonly TowerBaseSystem owner;

            public CombatState(TowerBaseSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {            
                owner.combatSystem.SetStartState();
            }

            public void Execute()
            {
                if (owner.RangeSystem.CreepList.Count > 0)
                {
                    if (owner.RangeSystem.CreepList[0] != null)
                    {
                        owner.RotateAtCreep();
                        owner.combatSystem.State.Update();
                    }

                    for (int i = 0; i < owner.RangeSystem.CreepList.Count; i++)
                    {
                        if (owner.RangeSystem.CreepList[i] == null)
                        {
                            owner.RangeSystem.CreepList.RemoveAt(i);
                            owner.RangeSystem.CreepSystemList.RemoveAt(i);
                        }
                    }                                 
                }
                else
                {
                    owner.state.ChangeState(new MoveRemainingBulletState(owner));
                }
            }

            public void Exit()
            {
            }
        }

        protected class MoveRemainingBulletState : IState
        {
            private readonly TowerBaseSystem owner;

            public MoveRemainingBulletState(TowerBaseSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
            }

            public void Execute()
            {
                if (!(owner.RangeSystem.CreepList.Count > 0))
                {
                    if (!owner.combatSystem.CheckAllBulletInactive())
                    {
                        owner.combatSystem.MoveBullet();
                    }
                    else
                    {
                        owner.state.ChangeState(new LookForCreepState(owner));
                    }
                }
                else
                {
                    owner.state.ChangeState(new CombatState(owner));
                }
            }

            public void Exit()
            {
            }
        }
    }
}
