﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;

namespace Game.Tower
{
    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public Transform RangeTransform, MovingPartTransform, ShootPointTransform;

        [HideInInspector]
        public GameObject OcuppiedCell, Range;        

        [HideInInspector]
        public TowerRangeSystem RangeSystem;     

        public TowerStats Stats;
        public GameObject Bullet, TowerPlaceEffect;

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

            state = new StateMachine();
            state.ChangeState(new SpawnState(this));

            Stats = Instantiate(Stats);

            combatSystem = GetComponent<TowerCombatSystem>();
            abilitySystem = GetComponent<TowerAbilitySystem>();

            Range = Instantiate(GM.Instance.RangePrefab, transform);
            RangeSystem = Range.GetComponent<TowerRangeSystem>();
            Range.transform.localScale = new Vector3(Stats.Range, 0.001f, Stats.Range);

            MovingPartTransform = transform.GetChild(0);
            ShootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);

            for (int i = 0; i < Stats.AbilityList.Count; i++)
            {
                Stats.AbilityList[i] = Instantiate(Stats.AbilityList[i]);

                for (int j = 0; j < Stats.AbilityList[i].EffectList.Count; j++)
                {
                    Stats.AbilityList[i].EffectList[j] = Instantiate(Stats.AbilityList[i].EffectList[j]); ;
                }

                Stats.AbilityList[i].SetOwnerTower(this);
            }

            rendererList = new List<Renderer>();
            rendererList.AddRange(GetComponentsInChildren<Renderer>());

            isRangeShowed = true;
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

            var placeEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90, 0, 0));
            Destroy(placeEffect, 1f);

            gameObject.layer = 14;
            RangeSystem.Show(false);
            isTowerPlaced = true;
        }

        private void RotateAtCreep()
        {
            var offset = RangeSystem.CreepInRangeList[0].transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            MovingPartTransform.rotation = Quaternion.Lerp(MovingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }

        public void Sell()
        {
            OcuppiedCell.GetComponent<TowerCells.Cell>().IsBusy = false;
            GM.Instance.TowerList.Remove(gameObject);
            Destroy(gameObject);
        }    

        public void AddExp(int amount)
        {
            Stats.Exp += amount;
            Debug.Log($"Added {amount} exp");

            if(Stats.Exp >= GM.ExpToLevelUp[Stats.Level - 1] && Stats.Level < 25)
            {
                state.ChangeState(new LevelUpState(this));
            }
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
                if (GM.PLAYERSTATE == GM.PLAYERSTATE_PLACINGTOWER)
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
                if (owner.RangeSystem.CreepInRangeList.Count > 0)
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
            }

            public void Execute()
            {
                for (int i = 0; i < owner.RangeSystem.CreepInRangeList.Count; i++)
                {
                    if (owner.RangeSystem.CreepInRangeList[i] == null)
                    {
                        owner.RangeSystem.CreepInRangeList.RemoveAt(i);
                        owner.RangeSystem.CreepInRangeSystemList.RemoveAt(i);
                    }                 
                }

                if (owner.RangeSystem.CreepInRangeList.Count <= 0)
                {
                    owner.state.ChangeState(new MoveRemainingBulletState(owner));
                }
                else if (owner.RangeSystem.CreepInRangeList[0] != null)
                {                 
                    owner.RotateAtCreep();
                    owner.combatSystem.Shoot(owner.Stats.AttackSpeed);                                     
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
                if (!owner.combatSystem.CheckAllBulletInactive())
                {
                    owner.combatSystem.MoveBulletOutOfRange();
                }
                else
                {
                    owner.state.ChangeState(new LookForCreepState(owner));
                }
            }

            public void Exit()
            {
            }
        }

        protected class LevelUpState : IState
        {
            private readonly TowerBaseSystem owner;

            public LevelUpState(TowerBaseSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                var stats = owner.Stats;

                for (int i = stats.Level; i < 25; i++)
                {
                    if (stats.Exp >= GM.ExpToLevelUp[stats.Level - 1] && stats.Level < 25)
                    {
                        Debug.Log($"LevelUp from {stats.Level} to {stats.Level + 1}");
                        stats.Level++;
                    }
                }
                
                owner.state.ChangeState(new CombatState(owner));
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
