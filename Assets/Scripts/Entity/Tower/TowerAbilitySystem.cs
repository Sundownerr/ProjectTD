using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class TowerAbilitySystem : ExtendedMonoBehaviour
    {
        public StateMachine state;

        private TowerBaseSystem towerBaseSystem;
        private List<Data.Ability> stackedAbilityList;
        private bool isAllEffectsEnded, isAllStackedEffectsEnded, isStackRequired;
        private int abilityStackRequiredIndex, stackCounter;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            towerBaseSystem = GetComponent<TowerBaseSystem>();
            stackedAbilityList = new List<Game.Data.Ability>();

            state = new StateMachine();
            state.ChangeState(new LookForCreepState(this));
        }

        protected class LookForCreepState : IState
        {
            TowerAbilitySystem owner;

            public LookForCreepState(TowerAbilitySystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                //Debug.Log("Look For Creep State");
            }

            public void Execute()
            {
                var tower = owner.towerBaseSystem;

                var isCreepInRange =
                    tower.RangeSystem.CreepList.Count > 0 &&
                    tower.RangeSystem.CreepList[0] != null;

                if (isCreepInRange)
                {
                    owner.state.ChangeState(new CombatState(owner));
                }
            }

            public void Exit()
            {
            }
        }

        protected class CreateStackAbilityState : IState
        {
            TowerAbilitySystem owner;

            public CreateStackAbilityState(TowerAbilitySystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                var tower = owner.towerBaseSystem;
                var abilityList = tower.StatsSystem.Stats.AbilityList;
                var stackList = owner.stackedAbilityList;

                stackList.Add(Instantiate(abilityList[owner.abilityStackRequiredIndex]));
                stackList[stackList.Count - 1].StackReset();

                //Debug.Log("Stacked: " + owner.stackedAbilityList.Count);

                owner.state.ChangeState(new CombatState(owner));
            }

            public void Execute()
            {
            }

            public void Exit()
            {
            }
        }


        protected class CombatState : IState
        {
            TowerAbilitySystem owner;

            public CombatState(TowerAbilitySystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
            }

            public void Execute()
            {
                var tower = owner.towerBaseSystem;
                var abilityList = owner.towerBaseSystem.StatsSystem.Stats.AbilityList;
                var stackList = owner.stackedAbilityList;

                var isCreepInRange =
                    tower.RangeSystem.CreepList.Count > 0 &&
                    tower.RangeSystem.CreepList[0] != null;

                if (isCreepInRange)
                {
                    for (int i = 0; i < abilityList.Count; i++)
                    {
                        abilityList[i].SetTarget(tower.RangeSystem.CreepSystemList);

                        abilityList[i].InitAbility();

                        if (abilityList[i].IsNeedStack)
                        {
                            owner.abilityStackRequiredIndex = i;
                            owner.state.ChangeState(new CreateStackAbilityState(owner));
                            abilityList[i].IsNeedStack = false;
                        }
                    }                 

                    if (stackList.Count > 0)
                    {
                        if (stackList[stackList.Count - 1].IsNeedStack)
                        {                          
                            owner.state.ChangeState(new CreateStackAbilityState(owner));
                            stackList[stackList.Count - 1].IsNeedStack = false;
                        }

                        for (int i = 0; i < stackList.Count; i++)
                        {
                            stackList[i].SetTarget(tower.RangeSystem.CreepSystemList);

                            stackList[i].InitAbility();

                            if (stackList[i].CheckEffectsEnded())
                            {
                                //Debug.Log($"Destroy{i}");
                                Destroy(stackList[i]);
                                stackList.RemoveAt(i);
                            }
                        }
                    }
                }           
                else
                {
                    for (int i = 0; i < abilityList.Count; i++)
                    {
                        if (!abilityList[i].CheckEffectsEnded() || !abilityList[i].CheckIntervalsEnded())
                        {
                            owner.state.ChangeState(new ContinueEffectState(owner));
                            owner.isAllEffectsEnded = false;
                        }
                    }

                    if (stackList.Count > 0)
                    {
                        for (int i = 0; i < stackList.Count; i++)
                        {
                            if (!stackList[i].CheckEffectsEnded() || !stackList[i].CheckIntervalsEnded())
                            {
                                owner.state.ChangeState(new ContinueEffectState(owner));
                                owner.isAllStackedEffectsEnded = false;
                            }
                        }
                    }

                    //Debug.Log("Going to Look For Creep State");
                    owner.state.ChangeState(new LookForCreepState(owner));
                }
            }

            public void Exit()
            {
            }
        }



        protected class ContinueEffectState : IState
        {
            TowerAbilitySystem owner;

            public ContinueEffectState(TowerAbilitySystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                //Debug.Log("Continu Effect State");
            }

            public void Execute()
            {
                var tower = owner.towerBaseSystem;
                var abilityList = tower.StatsSystem.Stats.AbilityList;
                var stackList = owner.stackedAbilityList;

                var isCreepInRange =
                    tower.RangeSystem.CreepList.Count > 0 &&
                    tower.RangeSystem.CreepList[0] != null;

                if ((owner.isAllEffectsEnded && owner.isAllStackedEffectsEnded) || isCreepInRange)
                {
                    owner.state.ChangeState(new CombatState(owner));
                }
                else
                {
                    owner.isAllEffectsEnded = true;

                    for (int i = 0; i < abilityList.Count; i++)
                    {
                        abilityList[i].InitAbility();

                        if (!abilityList[i].CheckEffectsEnded())
                        {
                            owner.isAllEffectsEnded = false;
                        }
                    }


                    if (stackList.Count > 0)
                    {
                        owner.isAllStackedEffectsEnded = true;

                        for (int i = 0; i < stackList.Count; i++)
                        {
                            stackList[i].InitAbility();

                            if (!stackList[i].CheckEffectsEnded())
                            {
                                owner.isAllStackedEffectsEnded = false;
                            }
                            else
                            {
                                Destroy(stackList[i]);
                                stackList.RemoveAt(i);
                            }                        
                        }
                    }
                }
            }

            public void Exit()
            {
                
            }
        }
    }
}