using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class TowerAbilitySystem 
    {
        public StateMachine State;

        private TowerBaseSystem tower;
        private List<Data.Ability> stackedAbilityList;
        private bool isAllEffectsEnded, isAllStackedEffectsEnded;
        private int abilityStackRequiredIndex;

        public TowerAbilitySystem(TowerBaseSystem ownerTower)
        {        
            tower = ownerTower;
          
            State = new StateMachine();
            State.ChangeState(new LookForCreepState(this));
        }

        public void Set()
        {
            stackedAbilityList = new List<Data.Ability>();
        }

        protected class LookForCreepState : IState
        {
            TowerAbilitySystem owner;

            public LookForCreepState(TowerAbilitySystem owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                var isCreepInRange =
                     owner.tower.RangeSystem.CreepList.Count > 0 &&
                     owner.tower.RangeSystem.CreepList[0] != null;

                if (isCreepInRange)
                    owner.State.ChangeState(new CombatState(owner));
                
            }

            public void Exit() { }
        }

        protected class CreateStackAbilityState : IState
        {
            TowerAbilitySystem owner;

            public CreateStackAbilityState(TowerAbilitySystem owner) { this.owner = owner; }

            public void Enter()
            {
                var abilityList = owner.tower.StatsSystem.Stats.AbilityList;
                var stackList = owner.stackedAbilityList;

                stackList.Add(Object.Instantiate(abilityList[owner.abilityStackRequiredIndex]));
                stackList[stackList.Count - 1].StackReset();
             
                owner.State.ChangeState(new CombatState(owner));
            }

            public void Execute() { }

            public void Exit() { }
        }


        protected class CombatState : IState
        {
            TowerAbilitySystem owner;

            public CombatState(TowerAbilitySystem owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                var abilityList = owner.tower.StatsSystem.Stats.AbilityList;
                var stackList = owner.stackedAbilityList;

                var isCreepInRange =
                     owner.tower.RangeSystem.CreepList.Count > 0 &&
                     owner.tower.RangeSystem.CreepList[0] != null;

                if (isCreepInRange)
                {
                    for (int i = 0; i < abilityList.Count; i++)
                    {
                        abilityList[i].SetTarget(owner.tower.RangeSystem.CreepSystemList);

                        abilityList[i].InitAbility();

                        if (abilityList[i].IsNeedStack)
                        {                        
                            owner.abilityStackRequiredIndex = i;
                            owner.State.ChangeState(new CreateStackAbilityState(owner));
                            abilityList[i].IsNeedStack = false;
                        }
                    }                 

                    if (stackList.Count > 0)
                    {
                        if (stackList[stackList.Count - 1].IsNeedStack)
                        {                          
                            owner.State.ChangeState(new CreateStackAbilityState(owner));
                            stackList[stackList.Count - 1].IsNeedStack = false;
                        }

                        for (int i = 0; i < stackList.Count; i++)
                        {
                            stackList[i].SetTarget(owner.tower.RangeSystem.CreepSystemList);

                            stackList[i].InitAbility();

                            if (stackList[i].CheckEffectsEnded())
                            {
                                Object.Destroy(stackList[i]);
                                stackList.RemoveAt(i);
                            }
                        }
                    }
                }           
                else
                {
                    for (int i = 0; i < abilityList.Count; i++)                   
                        if (!abilityList[i].CheckEffectsEnded())
                        {
                            owner.State.ChangeState(new ContinueEffectState(owner));
                            owner.isAllEffectsEnded = false;
                        }

                    if (stackList.Count > 0)
                        for (int i = 0; i < stackList.Count; i++)
                            if (!stackList[i].CheckEffectsEnded())
                            {
                                owner.State.ChangeState(new ContinueEffectState(owner));
                                owner.isAllStackedEffectsEnded = false;
                            }
                        
                    owner.State.ChangeState(new LookForCreepState(owner));
                }
            }

            public void Exit() { }
        }

        protected class ContinueEffectState : IState
        {
            TowerAbilitySystem owner;

            public ContinueEffectState(TowerAbilitySystem owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                var abilityList = owner.tower.StatsSystem.Stats.AbilityList;
                var stackList = owner.stackedAbilityList;

                var isCreepInRange =
                    owner.tower.RangeSystem.CreepList.Count > 0 &&
                    owner.tower.RangeSystem.CreepList[0] != null;

                var allEffectsEnded =
                    owner.isAllEffectsEnded &&
                    owner.isAllStackedEffectsEnded;

                if (allEffectsEnded || isCreepInRange)
                    owner.State.ChangeState(new CombatState(owner));               
                else
                {
                    owner.isAllEffectsEnded = true;
                    owner.isAllStackedEffectsEnded = true;

                    for (int i = 0; i < abilityList.Count; i++)
                    {
                        abilityList[i].InitAbility();

                        if (!abilityList[i].CheckEffectsEnded() && !abilityList[i].CheckIntervalsEnded())
                            owner.isAllEffectsEnded = false;
                    }                   

                    if (stackList.Count > 0)
                    {                      
                        for (int i = 0; i < stackList.Count; i++)
                        {
                            stackList[i].InitAbility();

                            if (!stackList[i].CheckEffectsEnded() && !stackList[i].CheckIntervalsEnded())
                                owner.isAllStackedEffectsEnded = false;
                            else
                            {
                                Object.Destroy(stackList[i]);
                                stackList.RemoveAt(i);
                            }                        
                        }
                    }
                }
            }

            public void Exit() { }
        }
    }
}