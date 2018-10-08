using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Base Ability")]
    
    public class Ability : ScriptableObject
    {
        [Expandable]
        public List<Effect.Effect> EffectList;

        [HideInInspector]
        public List<Creep.CreepSystem> creepDataList;

        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;
        public bool IsStackable, IsStacked, IsAbilityOnCooldown;
        public int EffectCount;
        
        private StateMachine state;


        private void Awake()
        {
            state = new StateMachine();
            state.ChangeState(new ChoseEffectState(this));
        }

        private void OnValidate()
        {
            var effectsOverallInterval = 0f;
            var effectsOverallDuration = 0f;

            for (int i = 0; i < EffectList.Count; i++)
            {
                effectsOverallInterval += EffectList[i].NextEffectInterval;
                effectsOverallDuration += EffectList[i].Duration;
            }

            if(effectsOverallInterval > Cooldown || effectsOverallDuration > Cooldown)
            {
                IsStackable = true;
            }
            else
            {
                IsStackable = false;
            }
        }

        public void InitAbility()
        {
            state.Update();
        }

        public void SetTarget(List<Creep.CreepSystem> data)
        {
            if (data != null && data.Count > 0 && creepDataList != data)
            {
                creepDataList = data;

                SetEffectData();
            }          
        }

        private void SetEffectData()
        {
            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i].CreepDataList = creepDataList;
            }
        }

        public void StackReset()
        {
            IsStacked = true;
            EffectCount = 0;
            IsAbilityOnCooldown = false;
            state.ChangeState(new ChoseEffectState(this));
        }

        private IEnumerator NextEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (EffectCount < EffectList.Count - 1)
            {
                state.ChangeState(new SetEffectState(this));
            }
            else
            {
                EffectCount = 0;
                state.ChangeState(new ChoseEffectState(this));
            }           
        }

        private IEnumerator AbilityCooldown(float delay)
        {
            IsAbilityOnCooldown = true;

            yield return new WaitForSeconds(delay);

            if (!IsStacked)
            {
                


                Debug.Log("NotStacked cd");
                IsAbilityOnCooldown = false;
            }
        }

        public class ChoseEffectState : IState
        {
            private Ability owner;

            public ChoseEffectState(Ability owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {               
            }

            public void Execute()
            {
                if (!owner.IsAbilityOnCooldown)
                {                    
                    if (owner.EffectCount < owner.EffectList.Count)
                    {
                        owner.state.ChangeState(new SetEffectState(owner));
                    }
                }
            }          

            public void Exit()
            {               
                GameManager.Instance.StartCoroutine(owner.AbilityCooldown(owner.Cooldown));
            }
        }

        public class SetEffectState : IState
        {
            private Ability owner;

            public SetEffectState(Ability owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {             
                GameManager.Instance.StartCoroutine(owner.NextEffect(owner.EffectList[owner.EffectCount].NextEffectInterval));               
            }

            public void Execute()
            {
                owner.EffectList[owner.EffectCount].InitEffect();
            }

            public void Exit()
            {
                owner.EffectCount++;
            }
        }       
    }
}