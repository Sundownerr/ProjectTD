using System.Collections.Generic;
using UnityEngine;
using Game.Data;
using Game.Systems;

namespace Game.Tower.System
{
    public class AbilitySystem
    {
        public StateMachine State;

        private TowerSystem tower;
        private List<Ability> abilityList, abilityStackList;
        private List<Effect> effectStackList;
        private bool isAllAbilitiesEnded, isAllAbilitiesStackEnded, isAllEffectsStackEnded;
        private int stackAbilityId, stackEffectId;

        public AbilitySystem(TowerSystem ownerTower)
        {
            tower = ownerTower;

            State = new StateMachine();
            State.ChangeState(new LookForCreepState(this));
        }

        public void Set()
        {
            abilityList = tower.Stats.AbilityList;
            abilityStackList    = new List<Ability>();
            effectStackList     = new List<Effect>();
        }

        private bool CheckTargetInRange(EntitySystem target)
        {
            for (int i = 0; i < tower.GetCreepInRangeList().Count; i++)
                if (target == tower.GetCreepInRangeList()[i])
                    return true;

            return false;
        }

        private bool CheckEffectTarget(EntitySystem target)
        {           
            if (effectStackList.Count > 0)
                for (int i = 0; i < effectStackList.Count; i++)
                    if (effectStackList[i].GetTarget() == target)               
                        for (int j = 0; j < effectStackList.Count; j++)                         
                            if(effectStackList[j] == effectStackList[i])
                                    return true;       
            return false;                  
        }

        protected class LookForCreepState : IState
        {
            private readonly AbilitySystem o;

            public LookForCreepState(AbilitySystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                var isCreepInRange =
                     o.tower.GetCreepInRangeList().Count > 0;

                if (isCreepInRange)
                    o.State.ChangeState(new CombatState(o));
            }

            public void Exit() { }
        }

        protected class CreateStackEffectState : IState
        {
            private AbilitySystem o;

            public CreateStackEffectState(AbilitySystem o) => this.o = o;

            public void Enter()
            {
                if (!o.CheckEffectTarget(o.abilityList[o.stackAbilityId].GetTarget()))
                {
                    o.effectStackList.Add(Object.Instantiate(o.abilityList[o.stackAbilityId].EffectList[o.stackEffectId]));
                    o.effectStackList[o.effectStackList.Count - 1].RestartState();
                    o.effectStackList[o.effectStackList.Count - 1].SetTarget(o.abilityList[o.stackAbilityId].GetTarget(), true);
                }
                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CreateStackAbilityState : IState
        {
            private AbilitySystem o;

            public CreateStackAbilityState(AbilitySystem o) => this.o = o;

            public void Enter()
            {
                o.abilityStackList.Add(Object.Instantiate(o.abilityList[o.stackAbilityId]));
                o.abilityStackList[o.abilityStackList.Count - 1].StackReset();
                o.abilityStackList[o.abilityStackList.Count - 1].SetTarget(o.abilityList[o.stackAbilityId].GetTarget());

                o.abilityList[o.stackAbilityId].IsNeedStack = false;

                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CombatState : IState
        {
            private readonly AbilitySystem o;

            public CombatState(AbilitySystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                var isCreepInRange =
                     o.tower.GetCreepInRangeList().Count > 0;

                if (isCreepInRange)
                {
                    for (int i = 0; i < o.abilityList.Count; i++)
                    {
                        if (!o.abilityList[i].IsOnCooldown)
                            for (int j = 0; j < o.abilityList[i].EffectList.Count; j++)
                                if (!o.abilityList[i].EffectList[j].IsStackable)
                                {
                                    var effect = o.abilityList[i].EffectList[j];
                                    var isNotAbilityTarget = 
                                        effect.GetTarget() != o.abilityList[i].GetTarget() || 
                                        !o.CheckTargetInRange((Creep.CreepSystem)effect.GetTarget());

                                    if (isNotAbilityTarget)
                                    {
                                        o.stackAbilityId = i;
                                        o.stackEffectId = j;
                                        o.State.ChangeState(new CreateStackEffectState(o));
                                    }
                                }

                        if (o.abilityList[i].GetTarget() != null && o.CheckTargetInRange(o.abilityList[i].GetTarget()))
                            o.abilityList[i].Init();
                        else
                            o.abilityList[i].SetTarget(o.tower.GetCreepInRangeList()[0]);

                        if (o.abilityList[i].IsNeedStack)
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

                    for (int i = 0; i < o.abilityList.Count; i++)
                        if (!o.abilityList[i].CheckAllEffectsEnded())
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
            private readonly AbilitySystem o;

            public ContinueEffectState(AbilitySystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                
                var isCreepInRange =
                    o.tower.GetCreepInRangeList().Count > 0;

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

                    for (int i = 0; i < o.abilityList.Count; i++)
                        if (o.abilityList[i].GetTarget() != null && !o.abilityList[i].CheckAllEffectsEnded())
                        {
                            o.abilityList[i].Init();
                            o.isAllAbilitiesEnded = false;
                        }
                        else
                            o.abilityList[i].CooldownReset();

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