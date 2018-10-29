using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Data/Tower/Ability")]

    [Serializable]
    public class Ability : ScriptableObject
    {       
        [HideInInspector]
        public bool IsNeedStack;

        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect.Effect> EffectList;

        private List<Creep.CreepSystem> creepList;
        private bool isStackable, isStacked, isOnCooldown, isChangingEffect;
        private StateMachine state;
        private Tower.TowerBaseSystem tower;
        private int effectCount;

        private void OnEnable()
        {
            CheckStackable();

            state = new StateMachine();
            state.ChangeState(new ChoseEffectState(this));
        }

        public void InitAbility()
        {
            state.Update();
        }

        public void SetOwnerTower(Tower.TowerBaseSystem ownerTower)
        {
            tower = ownerTower;

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].tower = ownerTower;
        }

        public void SetTarget(List<Creep.CreepSystem> creepList)
        {
            var isCreepListOk = 
                creepList.Count > 0 &&
                this.creepList != creepList;

            if (isCreepListOk)
            {
                this.creepList = creepList;

                for (int i = 0; i < EffectList.Count; i++)
                    EffectList[i].CreepList = creepList;
            }
        }
        
        public void StackReset()
        {      
            isStacked = true;
            effectCount = 0;
            isOnCooldown = false;

            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i] = Instantiate(EffectList[i]);
                EffectList[i].ResetEffect();
            }

            state.ChangeState(new ChoseEffectState(this));
        }

        private void CheckStackable()
        {
            var allEffectsInterval = 0f;
            var allEffectsDuration = 0f;

            for (int i = 0; i < EffectList.Count; i++)
            {
                allEffectsInterval += EffectList[i].NextEffectInterval;
                allEffectsDuration += EffectList[i].Duration;
            }
            
            if ((allEffectsInterval > Cooldown) || (allEffectsDuration > Cooldown))
                isStackable = true;
            else
                isStackable = false;      
        }

        public bool CheckEffectsEnded()
        {
            for (int i = 0; i < EffectList.Count; i++)
                if (!EffectList[i].IsEnded)
                    return false;
            
            return true;
        }

        public bool CheckIntervalsEnded()
        {
            return effectCount >= EffectList.Count ? true : false;         
        }

        private IEnumerator NextEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            isChangingEffect = false;

            state.ChangeState(new SetEffectState(this));
        }

        private IEnumerator StartCooldown(float delay)
        {         
            yield return new WaitForSeconds(delay);

            IsNeedStack = (isStackable && !CheckEffectsEnded()) ? true : false; 
                    
            if (!isStacked)
            {
                isOnCooldown = false;

                if (!CheckEffectsEnded())
                    state.ChangeState(new SetEffectState(this));
                else
                {
                    for (int i = 0; i < EffectList.Count; i++)
                        EffectList[i].ResetEffect();
              
                    effectCount = 0;

                    state.ChangeState(new ChoseEffectState(this));
                }              
            }
        }

        protected class ChoseEffectState : IState
        {
            private Ability owner;

            public ChoseEffectState(Ability owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                if (!owner.isOnCooldown && !owner.CheckIntervalsEnded())
                {
                    GM.Instance.StartCoroutine(owner.StartCooldown(owner.Cooldown));
                    owner.isOnCooldown = true;
                    owner.state.ChangeState(new SetEffectState(owner));
                }
            }

            public void Exit() { }
        }

        protected class SetEffectState : IState
        {
            private Ability owner;

            public SetEffectState(Ability owner) { this.owner = owner; }

            public void Enter() { }

            public void Execute()
            {
                if (!owner.isChangingEffect && !owner.CheckIntervalsEnded())
                {                   
                    GM.Instance.StartCoroutine(owner.NextEffect(owner.EffectList[owner.effectCount].NextEffectInterval));
                    owner.isChangingEffect = true;                  
                }

                if (owner.effectCount == 0)
                    owner.EffectList[0].InitEffect();
                else
                    for (int i = 0; i < owner.effectCount; i++)                 
                        owner.EffectList[i].InitEffect();
                              
                if (owner.CheckEffectsEnded())
                {
                    if (!owner.isStacked)
                        owner.state.ChangeState(new ChoseEffectState(owner));                   
                }               
            }

            public void Exit()
            {
                if (!owner.CheckIntervalsEnded())
                    owner.effectCount++;              
            }
        }        
    }
}