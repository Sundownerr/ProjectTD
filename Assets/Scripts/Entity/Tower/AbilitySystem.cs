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
        private bool isAllEffectsEnded, isAllAbilitiesStackEnded, isAllEffectsStackEnded, isInContinueState;
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
                    if (effectStackList[i].Target == target)               
                        for (int j = 0; j < effectStackList.Count; j++)                         
                            if(effectStackList[j].Id == effectStackList[i].Id)
                                    return true;       
            return false;                  
        }

        private void Init(Entity entity, bool condition)
        {                   
            if(entity is Ability ability)
                if (ability.Target != null && condition)
                {
                    isAllEffectsEnded = false;
                    ability.Init();                  
                }
                else if (!ability.IsStacked) 
                        if(!isInContinueState)
                            ability.SetTarget(tower.GetCreepInRangeList()[0]);
                        else
                            ability.CooldownReset();
                    else
                    {
                        for (int i = 0; i < ability.EffectList.Count; i++)                       
                            Object.Destroy(ability.EffectList[i]);
                                         
                        ability.EffectList.Clear();
                        Object.Destroy(ability);
                        abilityStackList.Remove(ability);
                    }      
            else if (entity is Effect effect)        
                if (effect.Target != null && condition)
                {
                    isAllEffectsEnded = false;
                    effect.Init();
                }
                else 
                {
                    Object.Destroy(effect);
                    effectStackList.Remove(effect);
                }          
        }

        private void CheckContinueEffects(Entity entity) 
        {
            var isNeedToContinue = 
                entity is Ability ability ? !ability.CheckAllEffectsEnded() :
                entity is Effect effect ? !effect.IsEnded : false;
            
            isAllEffectsEnded = !isNeedToContinue;

            if(isNeedToContinue)
                State.ChangeState(new ContinueEffectState(this));                 
        }

        protected class LookForCreepState : IState
        {
            private readonly AbilitySystem o;

            public LookForCreepState(AbilitySystem o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                if (o.tower.GetCreepInRangeList().Count > 0)
                    o.State.ChangeState(new CombatState(o));
            }

            public void Exit() { }
        }

        protected class CreateStackEffectState : IState
        {
            private readonly AbilitySystem o;

            public CreateStackEffectState(AbilitySystem o) => this.o = o;

            public void Enter()
            {
                if (!o.CheckEffectTarget(o.abilityList[o.stackAbilityId].Target))
                {
                    o.effectStackList.Add(Object.Instantiate(o.abilityList[o.stackAbilityId].EffectList[o.stackEffectId]));
                    o.effectStackList[o.effectStackList.Count - 1].RestartState();
                    o.effectStackList[o.effectStackList.Count - 1].SetTarget(o.abilityList[o.stackAbilityId].Target, true);
                }
                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

        protected class CreateStackAbilityState : IState
        {
            private readonly AbilitySystem o;

            public CreateStackAbilityState(AbilitySystem o) => this.o = o;

            public void Enter()
            {
                o.abilityStackList.Add(Object.Instantiate(o.abilityList[o.stackAbilityId]));
                o.abilityStackList[o.abilityStackList.Count - 1].StackReset();
                o.abilityStackList[o.abilityStackList.Count - 1].SetTarget(o.abilityList[o.stackAbilityId].Target);

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
                if (o.tower.GetCreepInRangeList().Count > 0)
                {
                    for (int i = 0; i < o.abilityList.Count; i++)
                    {
                        if (!o.abilityList[i].IsOnCooldown)
                            for (int j = 0; j < o.abilityList[i].EffectList.Count; j++)
                                if (!o.abilityList[i].EffectList[j].IsStackable)
                                {
                                    var effect = o.abilityList[i].EffectList[j];
                                    var isNotAbilityTarget = 
                                        effect.Target != o.abilityList[i].Target || 
                                        !o.CheckTargetInRange(effect.Target);

                                    if (isNotAbilityTarget)
                                    {
                                        o.stackAbilityId = i;
                                        o.stackEffectId = j;
                                        o.State.ChangeState(new CreateStackEffectState(o));
                                    }
                                }                                                        

                        if (o.abilityList[i].IsNeedStack)
                        {
                            o.stackAbilityId = i;
                            o.State.ChangeState(new CreateStackAbilityState(o));
                        }

                        o.Init(o.abilityList[i], o.CheckTargetInRange(o.abilityList[i].Target));   
                    }
                  
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityList[i], !o.abilityStackList[i].CheckAllEffectsEnded());                          
                
                    for (int i = 0; i < o.effectStackList.Count; i++)
                        o.Init(o.effectStackList[i], !o.effectStackList[i].IsEnded);                            
                }
                else
                {
                    o.isAllEffectsEnded = true;

                    for (int i = 0; i < o.abilityList.Count; i++)
                        o.CheckContinueEffects(o.abilityList[i]);                                        
                                                              
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                         o.CheckContinueEffects(o.abilityStackList[i]);
                                           
                    for (int i = 0; i < o.effectStackList.Count; i++)
                        o.CheckContinueEffects(o.effectStackList[i]);              

                    if (o.isAllEffectsEnded)
                        o.State.ChangeState(new LookForCreepState(o));
                }
            }

            public void Exit() { }
        }

        protected class ContinueEffectState : IState
        {          
            private readonly AbilitySystem o;

            public ContinueEffectState(AbilitySystem o) => this.o = o;

            public void Enter() => o.isAllEffectsEnded = false;          
            
            public void Execute()
            {         
                if (o.tower.GetCreepInRangeList().Count > 0 || o.isAllEffectsEnded)
                    o.State.ChangeState(new CombatState(o));
                else
                {
                    o.isAllEffectsEnded = true;
                   
                    for (int i = 0; i < o.abilityList.Count; i++)
                        o.Init(o.abilityList[i], !o.abilityList[i].CheckAllEffectsEnded());                   
                   
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityStackList[i], !o.abilityStackList[i].CheckAllEffectsEnded());                           
                
                    for (int i = 0; i < o.effectStackList.Count; i++)
                        o.Init(o.effectStackList[i], !o.effectStackList[i].IsEnded);
                }
            }

            public void Exit() { }
        }
    }
}