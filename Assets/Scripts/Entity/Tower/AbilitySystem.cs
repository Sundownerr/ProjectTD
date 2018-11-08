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
        private bool isAllEffectsEnded, isInContinueState;

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
        }

        private bool CheckTargetInRange(EntitySystem target)
        {
            for (int i = 0; i < tower.GetCreepInRangeList().Count; i++)
                if (target == tower.GetCreepInRangeList()[i])
                    return true;

            return false;
        }

        private void Init(Ability ability, bool condition)
        {                        
            if (ability.Target != null && condition)
            {
                isAllEffectsEnded = false;
                ability.Init();                  
            }
            else            
            { 
                if (!ability.IsStacked)                 
                    if(!isInContinueState)
                        ability.SetTarget(tower.GetCreepInRangeList()[0]);                       
                    else
                        ability.CooldownReset();                              
                else
                {
                    for (int i = 0; i < ability.EffectList.Count; i++)                       
                        Object.Destroy(ability.EffectList[i]);
                                        
                    ability.EffectList.Clear();                   
                    abilityStackList.Remove(ability);
                    Object.Destroy(ability);
                }          

                isAllEffectsEnded = true; 
            }            
        }

        private void CheckContinueEffects(Ability ability) 
        {                   
            if(!ability.CheckAllEffectsEnded())        
                State.ChangeState(new ContinueEffectState(this));                           
        }

        protected class LookForCreepState : IState
        {
            private readonly AbilitySystem o;

            public LookForCreepState(AbilitySystem o) => this.o = o;

            public void Enter() {Debug.Log("look state"); }

            public void Execute()
            {
                if (o.tower.GetCreepInRangeList().Count > 0)
                    o.State.ChangeState(new CombatState(o));
            }

            public void Exit() { }
        }

        protected class CreateStackState : IState
        {
            private readonly AbilitySystem o;
            private int stackAbilityId;

            public CreateStackState(AbilitySystem o, int stackAbilityId) 
            {
                this.o = o;
                this.stackAbilityId = stackAbilityId;
            }

            public void Enter()
            {             
                var stack = Object.Instantiate(o.abilityList[stackAbilityId]);
                                
                stack.StackReset(o.tower);           
                stack.SetTarget(o.abilityList[stackAbilityId].Target);

                o.abilityStackList.Add(stack);                 
                o.abilityList[stackAbilityId].IsNeedStack = false;
            
                o.State.ChangeState(new CombatState(o));
            }

            public void Execute() { }

            public void Exit() { }
        }

       
        protected class CombatState : IState
        {
            private readonly AbilitySystem o;

            public CombatState(AbilitySystem o) => this.o = o;

            public void Enter() 
            { 
                o.isInContinueState = false;       
            }

            public void Execute()
            {
                if (o.tower.GetCreepInRangeList().Count > 0)
                {
                    for (int i = 0; i < o.abilityList.Count; i++)
                    {                   
                        if (o.abilityList[i].IsNeedStack)                       
                            o.State.ChangeState(new CreateStackState(o, i));     
                            
                        o.Init(o.abilityList[i], o.CheckTargetInRange(o.abilityList[i].Target));                                     
                    }                  
                      
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityStackList[i], !o.abilityStackList[i].CheckAllEffectsEnded());                                                               
                }
                else
                {
                    o.isAllEffectsEnded = true;

                    for (int i = 0; i < o.abilityList.Count; i++)
                        o.CheckContinueEffects(o.abilityList[i]);                                        
                                                              
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.CheckContinueEffects(o.abilityStackList[i]);
                                   
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
                if(o.tower.GetCreepInRangeList().Count > 0)
                    o.State.ChangeState(new CombatState(o));

                if (o.isAllEffectsEnded)
                    o.State.ChangeState(new LookForCreepState(o));
                else
                {
                    o.isAllEffectsEnded = true;
                   
                    for (int i = 0; i < o.abilityList.Count; i++)
                        o.Init(o.abilityList[i], !o.abilityList[i].CheckAllEffectsEnded());                   
                   
                    for (int i = 0; i < o.abilityStackList.Count; i++)
                        o.Init(o.abilityStackList[i], !o.abilityStackList[i].CheckAllEffectsEnded());                        
                }
            }

            public void Exit() => o.isInContinueState = false;
        }
    }
}