using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class AbilityControlSystem : ITowerSystem
    {
        private StateMachine state;
        private TowerSystem tower;
        private List<AbilitySystem> abilitySystemList, abilityStackList;
        private bool isAllEffectsEnded, isInContinueState;

        public AbilityControlSystem(TowerSystem ownerTower)
        {
            tower = ownerTower;

            state = new StateMachine();
            state.ChangeState(new LookForCreepState(this));
        }

        public void Set()
        {
            abilitySystemList = tower.AbilitySystemList;
            abilityStackList = new List<AbilitySystem>();
        }

        public void Update() => state.Update();

        private bool CheckTargetInRange(EntitySystem target)
        {
            for (int i = 0; i < tower.CreepInRangeList.Count; i++)
                if (target == tower.CreepInRangeList[i])
                    return true;

            return false;
        }

        private void Init(AbilitySystem abilitySystem, bool condition)
        {
            if (abilitySystem.Target != null && condition)
            {
                isAllEffectsEnded = false;
                abilitySystem.Init();
            }
            else
            {
                if (!abilitySystem.IsStacked)
                    if (!isInContinueState)
                        abilitySystem.SetTarget(tower.CreepInRangeList[0]);
                    else
                    {
                        abilitySystem.CooldownReset();
                        abilitySystem.SetTarget(null);
                    }
                else
                {
                    for (int i = 0; i < abilitySystem.EffectSystemList.Count; i++)
                        abilitySystem.EffectSystemList.Remove(abilitySystem.EffectSystemList[i]);

                    abilitySystem.EffectSystemList.Clear();
                    abilityStackList.Remove(abilitySystem);
                }

                isAllEffectsEnded = true;
            }
        }

        private void CheckContinueEffects(AbilitySystem abilitySystem)
        {
            if (!abilitySystem.CheckAllEffectsEnded())
                state.ChangeState(new ContinueEffectState(this));
        }

        protected class LookForCreepState : IState
        {
            private readonly AbilityControlSystem o;

            public LookForCreepState(AbilityControlSystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                if (o.tower.CreepInRangeList.Count > 0)
                    o.state.ChangeState(new CombatState(o));
            }

            public void Exit() { }
        }

        protected class CreateStackState : IState
        {
            private readonly AbilityControlSystem o;
            private int stackAbilityId;

            public CreateStackState(AbilityControlSystem o, int stackAbilityId)
            {
                this.o = o;
                this.stackAbilityId = stackAbilityId;
            }

            public void Enter()
            {
                var stack =  new AbilitySystem(o.tower.Stats.AbilityList[stackAbilityId], o.tower);

                stack.StackReset(o.tower);
                stack.SetTarget(o.abilitySystemList[stackAbilityId].Target);

                o.abilityStackList.Add(stack);
                o.abilitySystemList[stackAbilityId].IsNeedStack = false;

                o.state.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CombatState : IState
        {
            private readonly AbilityControlSystem o;

            public CombatState(AbilityControlSystem o) => this.o = o;

            public void Enter()
            {
                o.isInContinueState = false;
            }

            public void Execute()
            {
                if (o.tower.CreepInRangeList.Count > 0)
                {
                    for (int i = 0; i < o.abilitySystemList.Count; i++)
                    {
                        if (o.abilitySystemList[i].IsNeedStack)
                            o.state.ChangeState(new CreateStackState(o, i));

                        o.Init(o.abilitySystemList[i], o.CheckTargetInRange(o.abilitySystemList[i].Target));
                    }

                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityStackList[i], !o.abilityStackList[i].CheckAllEffectsEnded());
                }
                else
                {
                    o.isAllEffectsEnded = true;

                    for (int i = 0; i < o.abilitySystemList.Count; i++)
                        o.CheckContinueEffects(o.abilitySystemList[i]);

                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.CheckContinueEffects(o.abilityStackList[i]);

                    if (o.isAllEffectsEnded)
                        o.state.ChangeState(new LookForCreepState(o));
                }
            }

            public void Exit() { }
        }

        protected class ContinueEffectState : IState
        {
            private readonly AbilityControlSystem o;

            public ContinueEffectState(AbilityControlSystem o) => this.o = o;

            public void Enter()
            {
                o.isAllEffectsEnded = false;
                o.isInContinueState = true;
            }

            public void Execute()
            {
                if (o.tower.CreepInRangeList.Count > 0)
                    o.state.ChangeState(new CombatState(o));

                if (o.isAllEffectsEnded)
                    o.state.ChangeState(new LookForCreepState(o));
                else
                {
                    o.isAllEffectsEnded = true;

                    for (int i = 0; i < o.abilitySystemList.Count; i++)
                        o.Init(o.abilitySystemList[i], !o.abilitySystemList[i].CheckAllEffectsEnded());

                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityStackList[i], !o.abilityStackList[i].CheckAllEffectsEnded());
                }
            }

            public void Exit() => o.isInContinueState = false;
        }
    }
}