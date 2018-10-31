using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class TowerAbilitySystem 
    {
        public StateMachine State;

        private TowerBaseSystem tower;
        private List<Data.Ability> stackList;
        private bool isAllEffectsEnded, isAllStackedEffectsEnded;
        private int stackRequiredIndex;

        public TowerAbilitySystem(TowerBaseSystem oTower)
        {        
            tower = oTower;
          
            State = new StateMachine();
            State.ChangeState(new LookForCreepState(this));
        }

        public void Set()
        {
            stackList = new List<Data.Ability>();
        }

        private bool CheckTargetInRange(Creep.CreepSystem target)
        {
            for (int i = 0; i < tower.RangeSystem.CreepSystemList.Count; i++)
                if (target == tower.RangeSystem.CreepSystemList[i])
                    return true;

            return false;
        }

        protected class LookForCreepState : IState
        {
            private readonly TowerAbilitySystem o;

            public LookForCreepState(TowerAbilitySystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                var isCreepInRange =
                     o.tower.RangeSystem.CreepList.Count > 0;

                if (isCreepInRange)
                    o.State.ChangeState(new CombatState(o));              
            }
            
            public void Exit() { }
        }

        protected class CreateStackState : IState
        {
            private TowerAbilitySystem o;

            public CreateStackState(TowerAbilitySystem o) { this.o = o; }

            public void Enter()
            {
                var abilityList = o.tower.StatsSystem.Stats.AbilityList;

                o.stackList.Add(Object.Instantiate(abilityList[o.stackRequiredIndex]));
                o.stackList[o.stackList.Count - 1].StackReset();
                o.stackList[o.stackList.Count - 1].SetTarget(abilityList[o.stackRequiredIndex].GetTarget());

                abilityList[o.stackRequiredIndex].IsNeedStack = false;

                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CombatState : IState
        {
            private readonly TowerAbilitySystem o;

            public CombatState(TowerAbilitySystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                var abilityList = o.tower.StatsSystem.Stats.AbilityList;

                var isCreepInRange =
                     o.tower.RangeSystem.CreepList.Count > 0;

                if (isCreepInRange)
                {
                    for (int i = 0; i < abilityList.Count; i++)
                    {
                        abilityList[i].SetAvailableTargetList(o.tower.RangeSystem.CreepSystemList);
                        
                        if (abilityList[i].GetTarget() != null && o.CheckTargetInRange(abilityList[i].GetTarget()))
                            abilityList[i].Init();
                        else
                            abilityList[i].SetTarget(o.tower.RangeSystem.CreepSystemList[0]);


                        if (abilityList[i].IsNeedStack)
                        {
                            o.stackRequiredIndex = i;
                            o.State.ChangeState(new CreateStackState(o));
                        }
                    }

                    if (o.stackList.Count > 0)
                        for (int i = 0; i < o.stackList.Count; i++)
                            if (o.stackList[i].GetTarget() != null && !o.stackList[i].CheckAllEffectsEnded())
                                o.stackList[i].Init();
                            else
                            {
                                Object.Destroy(o.stackList[i]);
                                for (int j = 0; j < o.stackList[i].EffectList.Count; j++)
                                    Object.Destroy(o.stackList[i].EffectList[j]);

                                o.stackList.RemoveAt(i);
                            }
                }
                else
                {                
                    o.isAllEffectsEnded = true;
                    o.isAllStackedEffectsEnded = true;

                    for (int i = 0; i < abilityList.Count; i++)
                        if (!abilityList[i].CheckAllEffectsEnded())
                        {
                            o.isAllEffectsEnded = false;
                            o.State.ChangeState(new ContinueEffectState(o));
                        }

                    if (o.stackList.Count > 0)
                        for (int i = 0; i < o.stackList.Count; i++)
                            if (!o.stackList[i].CheckAllEffectsEnded())
                            {
                                o.isAllStackedEffectsEnded = false;
                                o.State.ChangeState(new ContinueEffectState(o));
                            }

                    if (o.isAllEffectsEnded && o.isAllStackedEffectsEnded)
                        o.State.ChangeState(new LookForCreepState(o));
                }
            }

            public void Exit() { }
        }

        protected class ContinueEffectState : IState
        {
            private readonly TowerAbilitySystem o;

            public ContinueEffectState(TowerAbilitySystem o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {

                var abilityList = o.tower.StatsSystem.Stats.AbilityList;

                var isCreepInRange =
                    o.tower.RangeSystem.CreepList.Count > 0;

                var allEffectsEnded =
                    o.isAllEffectsEnded &&
                    o.isAllStackedEffectsEnded;

                if (allEffectsEnded || isCreepInRange)
                    o.State.ChangeState(new CombatState(o));
                else
                {
                    o.isAllEffectsEnded = true;
                    o.isAllStackedEffectsEnded = true;

                    for (int i = 0; i < abilityList.Count; i++)
                        if (abilityList[i].CheckAllEffectsEnded())
                            abilityList[i].Reset();
                        else
                        {
                            abilityList[i].Init();
                            o.isAllEffectsEnded = false;
                        }

                    if (o.stackList.Count > 0)
                        for (int i = 0; i < o.stackList.Count; i++)
                            if (!o.stackList[i].CheckAllEffectsEnded())
                            {
                                o.stackList[i].Init();
                                o.isAllStackedEffectsEnded = false;
                            }
                            else
                            {
                                Object.Destroy(o.stackList[i]);
                                for (int j = 0; j < o.stackList[i].EffectList.Count; j++)
                                    Object.Destroy(o.stackList[i].EffectList[j]);

                                o.stackList.RemoveAt(i);
                            }
                }
            }

            public void Exit() { }
        }
    }
}