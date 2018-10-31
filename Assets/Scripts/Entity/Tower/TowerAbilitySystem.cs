using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class TowerAbilitySystem
    {
        public StateMachine State;

        private TowerBaseSystem tower;
        private List<Data.Ability> abilityStackList;
        private List<Data.Effect.Effect> effectStackList;
        private bool isAllAbilitiesEnded, isAllAbilitiesStackEnded, isAllEffectsStackEnded;
        private int stackAbilityId, stackEffectId;

        public TowerAbilitySystem(TowerBaseSystem oTower)
        {
            tower = oTower;

            State = new StateMachine();
            State.ChangeState(new LookForCreepState(this));
        }

        public void Set()
        {
            abilityStackList = new List<Data.Ability>();
            effectStackList = new List<Data.Effect.Effect>();
        }

        private bool CheckTargetInRange(Creep.CreepSystem target)
        {
            for (int i = 0; i < tower.RangeSystem.CreepSystemList.Count; i++)
                if (target == tower.RangeSystem.CreepSystemList[i])
                    return true;

            return false;
        }

        private bool CheckEffectTarget(Creep.CreepSystem target)
        {
            if (effectStackList.Count > 0)
                for (int i = 0; i < effectStackList.Count; i++)
                    if (effectStackList[i].GetTarget() == target)
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

        protected class CreateStackEffectState : IState
        {
            private TowerAbilitySystem o;

            public CreateStackEffectState(TowerAbilitySystem o) { this.o = o; }

            public void Enter()
            {
                var abilityList = o.tower.StatsSystem.Stats.AbilityList;

                if (!o.CheckEffectTarget(abilityList[o.stackAbilityId].GetTarget()))
                {
                    o.effectStackList.Add(Object.Instantiate(abilityList[o.stackAbilityId].EffectList[o.stackEffectId]));
                    o.effectStackList[o.effectStackList.Count - 1].Reset();
                    o.effectStackList[o.effectStackList.Count - 1].SetTarget(abilityList[o.stackAbilityId].GetTarget());
                }
                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CreateStackAbilityState : IState
        {
            private TowerAbilitySystem o;

            public CreateStackAbilityState(TowerAbilitySystem o) { this.o = o; }

            public void Enter()
            {
                var abilityList = o.tower.StatsSystem.Stats.AbilityList;

                o.abilityStackList.Add(Object.Instantiate(abilityList[o.stackAbilityId]));
                o.abilityStackList[o.abilityStackList.Count - 1].StackReset();
                o.abilityStackList[o.abilityStackList.Count - 1].SetTarget(abilityList[o.stackAbilityId].GetTarget());

                abilityList[o.stackAbilityId].IsNeedStack = false;

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
                        if (!abilityList[i].IsOnCooldown)
                            for (int j = 0; j < abilityList[i].EffectList.Count; j++)
                                if (!abilityList[i].EffectList[j].IsStackable)
                                {
                                    var effect = abilityList[i].EffectList[j];

                                    if (effect.GetTarget() != abilityList[i].GetTarget() || !o.CheckTargetInRange(effect.GetTarget()))
                                    {
                                        o.stackAbilityId = i;
                                        o.stackEffectId = j;
                                        o.State.ChangeState(new CreateStackEffectState(o));
                                    }
                                }

                        if (abilityList[i].GetTarget() != null && o.CheckTargetInRange(abilityList[i].GetTarget()))
                            abilityList[i].Init();
                        else
                            abilityList[i].SetTarget(o.tower.RangeSystem.CreepSystemList[0]);

                        if (abilityList[i].IsNeedStack)
                        {
                            o.stackAbilityId = i;
                            o.State.ChangeState(new CreateStackAbilityState(o));
                        }
                    }

                    if (o.abilityStackList.Count > 0)
                        for (int i = 0; i < o.abilityStackList.Count; i++)
                            if (o.abilityStackList[i].GetTarget() != null && !o.abilityStackList[i].CheckAllEffectsEnded())
                                o.abilityStackList[i].Init();
                            else
                            {
                                Object.Destroy(o.abilityStackList[i]);
                                for (int j = 0; j < o.abilityStackList[i].EffectList.Count; j++)
                                {
                                    Object.Destroy(o.abilityStackList[i].EffectList[j]);
                                    o.abilityStackList[i].EffectList.RemoveAt(j);
                                }

                                o.abilityStackList.RemoveAt(i);
                            }

                    if (o.effectStackList.Count > 0)
                        for (int i = 0; i < o.effectStackList.Count; i++)
                            if (o.effectStackList[i].GetTarget() != null && !o.effectStackList[i].IsEnded)
                                o.effectStackList[i].Init();
                            else
                            {
                                Object.Destroy(o.effectStackList[i]);
                                o.effectStackList.RemoveAt(i);
                            }
                }
                else
                {
                    o.isAllAbilitiesEnded = true;
                    o.isAllAbilitiesStackEnded = true;
                    o.isAllEffectsStackEnded = true;

                    for (int i = 0; i < abilityList.Count; i++)
                        if (!abilityList[i].CheckAllEffectsEnded())
                        {
                            o.isAllAbilitiesEnded = false;
                            o.State.ChangeState(new ContinueEffectState(o));
                        }

                    if (o.abilityStackList.Count > 0)
                        for (int i = 0; i < o.abilityStackList.Count; i++)
                            if (!o.abilityStackList[i].CheckAllEffectsEnded())
                            {
                                o.isAllAbilitiesStackEnded = false;
                                o.State.ChangeState(new ContinueEffectState(o));
                            }

                    if (o.effectStackList.Count > 0)
                        for (int i = 0; i < o.effectStackList.Count; i++)
                            if (!o.effectStackList[i].IsEnded)
                            {
                                o.isAllEffectsStackEnded = false;
                                o.State.ChangeState(new ContinueEffectState(o));
                            }

                    if (o.isAllAbilitiesEnded && o.isAllAbilitiesStackEnded && o.isAllEffectsStackEnded)
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

                var isAllEnded =
                    o.isAllAbilitiesEnded &&
                    o.isAllAbilitiesStackEnded &&
                    o.isAllEffectsStackEnded;

                if (isCreepInRange || isAllEnded)
                    o.State.ChangeState(new CombatState(o));
                else
                {
                    o.isAllAbilitiesEnded = true;
                    o.isAllAbilitiesStackEnded = true;
                    o.isAllEffectsStackEnded = true;

                    for (int i = 0; i < abilityList.Count; i++)
                        if (abilityList[i].GetTarget() != null && !abilityList[i].CheckAllEffectsEnded())
                        {
                            abilityList[i].Init();
                            o.isAllAbilitiesEnded = false;
                        }
                        else
                            abilityList[i].Reset();

                    if (o.abilityStackList.Count > 0)
                        for (int i = 0; i < o.abilityStackList.Count; i++)
                            if (o.abilityStackList[i].GetTarget() != null && !o.abilityStackList[i].CheckAllEffectsEnded())
                            {
                                o.abilityStackList[i].Init();
                                o.isAllAbilitiesStackEnded = false;
                            }
                            else
                            {
                                Object.Destroy(o.abilityStackList[i]);
                                for (int j = 0; j < o.abilityStackList[i].EffectList.Count; j++)
                                {
                                    Object.Destroy(o.abilityStackList[i].EffectList[j]);
                                    o.abilityStackList[i].EffectList.RemoveAt(j);
                                }
                                o.abilityStackList.RemoveAt(i);
                            }

                    if (o.effectStackList.Count > 0)
                        for (int i = 0; i < o.effectStackList.Count; i++)
                            if (o.effectStackList[i].GetTarget() != null && !o.effectStackList[i].IsEnded)
                            {
                                o.effectStackList[i].Init();
                                o.isAllEffectsStackEnded = false;
                            }
                            else
                            {
                                Object.Destroy(o.effectStackList[i]);
                                o.effectStackList.RemoveAt(i);
                            }
                }
            }

            public void Exit() { }
        }
    }
}