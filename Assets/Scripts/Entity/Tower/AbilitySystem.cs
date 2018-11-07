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
        private List<Ability> abilityList, abilityStackList, singleEffectAbilityList;
        private bool isAllEffectsEnded, isInContinueState;
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
            abilityStackList        = new List<Ability>();
            singleEffectAbilityList = new List<Ability>();
        }

        private bool CheckTargetInRange(EntitySystem target)
        {
            for (int i = 0; i < tower.GetCreepInRangeList().Count; i++)
                if (target == tower.GetCreepInRangeList()[i])
                    return true;

            return false;
        }

        private bool CheckStackEffectTarget(EntitySystem target)
        {    
                      
            for (int i = 0; i < singleEffectAbilityList.Count; i++)
                for (int j = 0; j < singleEffectAbilityList[i].EffectList.Count; j++)
                {
                    var effect = singleEffectAbilityList[i].EffectList[j];

                    if (effect.Target == target)               
                        for (int k = 0; k < singleEffectAbilityList[k].EffectList.Count; k++)       
                            if(effect.CompareId(singleEffectAbilityList[i].EffectList[k].Id))
                                return true;                                                              
                }
                
            return false;                  
        }

        private void Init(Ability ability, bool condition)
        {                        
            if (ability.Target != null && condition)
            {
                isAllEffectsEnded = false;
                ability.Init();                  
            }
            else if (!ability.IsStacked) 
            {
                if(!isInContinueState)
                    ability.SetTarget(tower.GetCreepInRangeList()[0]);                       
                else
                    ability.CooldownReset();                 
            }
            else
            {
                for (int i = 0; i < ability.EffectList.Count; i++)                       
                    Object.Destroy(ability.EffectList[i]);
                                    
                ability.EffectList.Clear();
                Object.Destroy(ability);
                abilityStackList.Remove(ability);
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

        protected enum Stack
        {
            Ability,
            Effect
        }

        protected class CreateStackState : IState
        {
            private readonly AbilitySystem o;
            private Stack stackType;

            public CreateStackState(AbilitySystem o, Stack stackType) 
            {
                this.o = o;
                this.stackType = stackType;
            }

            public void Enter()
            {
                if(stackType == Stack.Ability)
                {
                    var stackList = o.abilityStackList;
                    var stack = Object.Instantiate(o.abilityList[o.stackAbilityId]);
                                 
                    stack.StackReset(o.tower);           
                    stack.SetTarget(o.abilityList[o.stackAbilityId].Target);

                    stackList.Add(stack);                 

                    o.abilityList[o.stackAbilityId].IsNeedStack = false;
                }
                else
                {
                    var stackAbility = ScriptableObject.CreateInstance<Ability>();                    
                    var stackEffect = Object.Instantiate(o.abilityList[o.stackAbilityId].EffectList[o.stackEffectId]);

                    stackAbility.IsStacked = true;
                    stackAbility.SetOwner(o.tower);

                    stackEffect.RestartState();
                    stackEffect.SetOwner(o.tower, stackAbility);                    
                    stackAbility.SetTarget(o.abilityList[o.stackAbilityId].Target);

                    stackAbility.EffectList.Add(stackEffect);                             
                    o.singleEffectAbilityList.Add(stackAbility);
                }

                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

       
        protected class CombatState : IState
        {
            private readonly AbilitySystem o;

            public CombatState(AbilitySystem o) => this.o = o;

            public void Enter() { o.isInContinueState = false;}

            public void Execute()
            {
                if (o.tower.GetCreepInRangeList().Count > 0)
                {
                    for (int i = 0; i < o.abilityList.Count; i++)
                    {                  
                        
                        o.Init(o.abilityList[i], o.CheckTargetInRange(o.abilityList[i].Target));   

                        if (!o.abilityList[i].IsOnCooldown)
                        {

                            for (int j = 0; j < o.abilityList[i].EffectList.Count; j++)
                                if (!o.abilityList[i].EffectList[j].IsStackable)
                                {
                                    var effect = o.abilityList[i].EffectList[j];

                                    Debug.Log(effect.Target);

                                    var isNotAbilityTarget = 
                                        !o.CheckTargetInRange(effect.Target) || 
                                        effect.Target != o.abilityList[i].Target;
                                        
                                    if (isNotAbilityTarget && !o.CheckStackEffectTarget(o.abilityList[i].Target))
                                    {
                                        o.stackAbilityId = i;
                                        o.stackEffectId = j;
                                        o.State.ChangeState(new CreateStackState(o, Stack.Effect));
                                    }
                                }      
                        }                                                  

                        if (o.abilityList[i].IsNeedStack)
                        {
                            o.stackAbilityId = i;
                            o.State.ChangeState(new CreateStackState(o, Stack.Ability));
                        }                                     
                    }                  
                      
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityStackList[i], !o.abilityStackList[i].CheckAllEffectsEnded());                          
                
                    for (int i = 0; i < o.singleEffectAbilityList.Count; i++)
                        o.Init(o.singleEffectAbilityList[i], !o.singleEffectAbilityList[i].CheckAllEffectsEnded());                                            
                }
                else
                {
                    o.isAllEffectsEnded = true;

                    for (int i = 0; i < o.abilityList.Count; i++)
                        o.CheckContinueEffects(o.abilityList[i]);                                        
                                                              
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.CheckContinueEffects(o.abilityStackList[i]);
                                           
                    for (int i = 0; i < o.singleEffectAbilityList.Count; i++)
                        o.CheckContinueEffects(o.singleEffectAbilityList[i]);              

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

            public void Enter() 
            {
                o.isAllEffectsEnded = false;  
                o.isInContinueState = true;
            }        
            
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
                
                    for (int i = 0; i < o.singleEffectAbilityList.Count; i++)
                        o.Init(o.singleEffectAbilityList[i], !o.singleEffectAbilityList[i].CheckAllEffectsEnded());
                }
            }

            public void Exit() => o.isInContinueState = false;
        }
    }
}