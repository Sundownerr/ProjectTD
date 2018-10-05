using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Tower;


namespace Game.Tower
{

    public class TowerBaseSystem : ExtendedMonoBehaviour
    {
      //  [HideInInspector]
        public Transform RangeTransform, MovingPartTransform, ShootPointTransform;

       // [HideInInspector]
        public GameObject OcuppiedCell, Range;

      //  [HideInInspector]
        public TowerCombatSystem CombatSystem;

       // [HideInInspector]
        public TowerRangeSystem RangeSystem;
       
       // [HideInInspector]
        public TowerStats Stats;

        public TowerStats BaseStats;
        public GameObject Bullet, TowerPlaceEffect, Target;

        protected List<Renderer> rendererList;
        private StateMachine state;
        private bool isRangeShowed;

        private void Start()
        {
            state = new StateMachine();
            state.ChangeState(new SpawnState(this));

            isRangeShowed = true;
        }

        private void Update()
        {
            state.Update();

            SetRangeShow();
        }

        private void SetRangeShow()
        {
            var isChoosedTower = 
                GameManager.Instance.TowerUISystem.gameObject.activeSelf && 
                GameManager.Instance.PlayerInputSystem.ChoosedTower == gameObject;

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
            SetTowerColor(GameManager.Instance.TowerPlaceSystem.GhostedTowerColor);

            transform.position = GameManager.Instance.TowerPlaceSystem.GhostedTowerPos;
        }

        private void EndPlacing()
        {
            OcuppiedCell = GameManager.Instance.TowerPlaceSystem.NewBusyCell;

            if (transform.position != OcuppiedCell.transform.position)
            {
                transform.position = OcuppiedCell.transform.position;
            }
            
            SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));

            var placeEffect = Instantiate(TowerPlaceEffect, transform.position + Vector3.up * 5, Quaternion.Euler(90, 0, 0));
            Destroy(placeEffect, 1f);

            gameObject.layer = 14;
            RangeSystem.Show(false);

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
            GameManager.Instance.TowerList.Remove(gameObject);
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
                owner.Stats = Instantiate(owner.BaseStats);
                owner.CombatSystem = owner.GetComponent<TowerCombatSystem>();

                owner.Range = Instantiate(GameManager.Instance.RangePrefab, owner.transform);
                owner.RangeSystem = owner.Range.GetComponent<TowerRangeSystem>();
                owner.Range.transform.localScale = new Vector3(owner.Stats.Range, 0.001f, owner.Stats.Range);

                owner.MovingPartTransform = owner.transform.GetChild(0);
                owner.ShootPointTransform = owner.MovingPartTransform.GetChild(0).GetChild(0);

                for (int i = 0; i < owner.Stats.TowerAbilityList.Count; i++)
                {
                    owner.Stats.TowerAbilityList[i] = Instantiate(owner.Stats.TowerAbilityList[i]);
                    owner.Stats.TowerAbilityList[i].tower.Add(owner.gameObject);
                    owner.Stats.TowerAbilityList[i].creep.Add(owner.Target);
                    owner.Stats.TowerAbilityList[i].GetData();
                }

                owner.rendererList = new List<Renderer>();
                owner.rendererList.AddRange(owner.GetComponentsInChildren<Renderer>());
            }

            public void Execute()
            {
                if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
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
                owner.Stats.TowerAbilityList[0].creepData = owner.RangeSystem.CreepInRangeList[0].GetComponent<Creep.CreepSystem>();
            }

            public void Execute()
            {
                for (int i = 0; i < owner.RangeSystem.CreepInRangeList.Count; i++)
                {
                    if (owner.RangeSystem.CreepInRangeList[i] == null)
                    {
                        owner.RangeSystem.CreepInRangeList.RemoveAt(i);
                    }
                }

                if (owner.RangeSystem.CreepInRangeList.Count <= 0)
                {
                    owner.state.ChangeState(new MoveRemainingBulletState(owner));
                }
                else if (owner.RangeSystem.CreepInRangeList[0] != null)
                {
                    owner.Target = owner.RangeSystem.CreepInRangeList[0];
                    owner.RotateAtCreep();
                    owner.CombatSystem.Shoot(owner.Stats.AttackSpeed);

                    owner.Stats.TowerAbilityList[0].InitAbility();
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
                if (!owner.CombatSystem.CheckAllBulletInactive())
                {
                    owner.CombatSystem.MoveBulletOutOfRange();
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
    }
}
