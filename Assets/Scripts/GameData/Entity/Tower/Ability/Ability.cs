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

        [HideInInspector]
        public bool IsStackable, IsStacked, IsNeedStack, IsAbilityOnCooldown;

        [HideInInspector]
        public int EffectCount;

        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;       
        
        private StateMachine state;
        private Tower.TowerBaseSystem ownerTower;

        private void Awake()
        {
            state = new StateMachine();
            state.ChangeState(new ChoseEffectState(this));

            var allEffectsInterval = 0f;
            var allEffectsDuration = 0f;

            for (int i = 0; i < EffectList.Count; i++)
            {
                allEffectsInterval += EffectList[i].NextEffectInterval;
                allEffectsDuration += EffectList[i].Duration;
            }

            if (allEffectsInterval > Cooldown || allEffectsDuration > Cooldown)
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

        public void SetOwnerTower(Tower.TowerBaseSystem ownerTower)
        {
            this.ownerTower = ownerTower;

            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i].ownerTower = ownerTower;
            }
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

            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i] = Instantiate(EffectList[i]);
                EffectList[i].StackReset();
            }

            state.ChangeState(new ChoseEffectState(this));
        }

        public bool CheckEffectsEnded()
        {
            for (int i = 0; i < EffectList.Count; i++)
            {
                if (!EffectList[i].IsEnded)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckIntervalsEnded()
        {
            if (EffectCount < EffectList.Count - 1)
            {
                return false;
            }
            return true;
        }

        private IEnumerator NextEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (EffectCount < EffectList.Count - 1)
            {
                EffectCount++;
                state.ChangeState(new SetEffectState(this));
                
            }
            else
            {
                if (!IsStacked)
                {
                    EffectCount = 0;
                   
                }
                else
                {
                    state.ChangeState(new SetEffectState(this));
                }
            }
        }

        private IEnumerator AbilityCooldown(float delay)
        {
            IsAbilityOnCooldown = true;

            yield return new WaitForSeconds(delay);

            if (IsStackable)
            {
                if (!CheckEffectsEnded() || !CheckIntervalsEnded())
                {
                    IsNeedStack = true;
                }
                else
                {
                    IsNeedStack = false;
                }
            }

            if (!IsStacked)
            {
                IsAbilityOnCooldown = false;
                state.ChangeState(new ChoseEffectState(this));
            }
        }

        protected class ChoseEffectState : IState
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
                    if (owner.EffectCount < owner.EffectList.Count - 1)
                    {
                        owner.state.ChangeState(new SetEffectState(owner));
                    }
                }
            }          

            public void Exit()
            {
                GM.Instance.StartCoroutine(owner.AbilityCooldown(owner.Cooldown));
                
            }
        }

        protected class SetEffectState : IState
        {
            private Ability owner;

            public SetEffectState(Ability owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                Debug.Log(owner.EffectList[owner.EffectCount].NextEffectInterval + " \n count: " + owner.EffectCount);
           
                GM.Instance.StartCoroutine(owner.NextEffect(owner.EffectList[owner.EffectCount].NextEffectInterval));
                
            }

            public void Execute()
            {
                if (owner.EffectCount > 1)
                {
                    for (int i = 0; i < owner.EffectCount; i++)
                    {
                        owner.EffectList[i].InitEffect();
                    }
                }
                else
                {
                    owner.EffectList[owner.EffectCount].InitEffect();
                }
            }

            public void Exit()
            {
             
            }
        }       
    }
}