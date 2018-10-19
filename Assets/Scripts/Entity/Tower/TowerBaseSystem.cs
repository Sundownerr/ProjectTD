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
        public Transform RangeTransform, MovingPartTransform, ShootPointTransform;

        [HideInInspector]
        public GameObject OcuppiedCell, Range;        

        [HideInInspector]
        public TowerRangeSystem RangeSystem;     

        public TowerData Stats;
        public GameObject Bullet, TowerPlaceEffect, LevelUpEffect;

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

            state = new StateMachine();
            state.ChangeState(new SpawnState(this));
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
        
        private void IncreaseStats()
        {           
            Stats.Damage += Mathf.FloorToInt(GetPercentOfValue(4f, Stats.Damage));
            Stats.AttackSpeed += GetPercentOfValue(1.2f, Stats.AttackSpeed);
            Stats.CritChance += GetPercentOfValue(0.2f, Stats.CritChance);
            Stats.SpellCritChance += GetPercentOfValue(0.2f, Stats.SpellCritChance);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (GM.Instance.PlayerInputSystem.ChoosedTower == gameObject)
            {
                GM.Instance.TowerUISystem.UpdateValues();
            }
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
            var offset = RangeSystem.CreepList[0].transform.position - transform.position;
            offset.y = 0;

            var towerRotation = Quaternion.LookRotation(offset);

            MovingPartTransform.rotation = Quaternion.Lerp(MovingPartTransform.rotation, towerRotation, Time.deltaTime * 9f);
        }

        public void Sell()
        {
            GM.Instance.ResourceSystem.AddTowerLimit(-Stats.TowerLimit);
            GM.Instance.ResourceSystem.AddGold(Stats.GoldCost);

            OcuppiedCell.GetComponent<TowerCells.Cell>().IsBusy = false;
            GM.Instance.TowerList.Remove(gameObject);
            Destroy(gameObject);
        }

        public void Upgrade()
        {
            
        }

        public void AddExp(int amount)
        {
            Stats.Exp += amount;

            for (int i = Stats.Level; i < 25; i++)
            {
                if (Stats.Exp >= GM.ExpToLevelUp[Stats.Level - 1] && Stats.Level < 25)
                {
                    IncreaseStats();
                    Stats.Level++;

                    var effect = Instantiate(GM.Instance.LevelUpEffect, transform.position, Quaternion.identity);
                    Destroy(effect, 2f);
                }
            }
            UpdateUI();
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
