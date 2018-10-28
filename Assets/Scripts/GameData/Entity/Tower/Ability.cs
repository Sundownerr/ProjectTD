using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Base Ability")]

    [Serializable]
    public class Ability : ScriptableObject
    {       
        [HideInInspector]
        public List<Creep.CreepSystem> CreepList;

        [HideInInspector]
        public bool IsStackable, IsStacked, IsNeedStack, IsOnCooldown, IsChangingEffect;

        [HideInInspector]
        public int EffectCount;

        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect.Effect> EffectList;

        private StateMachine state;
        private Tower.TowerBaseSystem tower;

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
            {
                EffectList[i].tower = ownerTower;
            }
        }

        public void SetTarget(List<Creep.CreepSystem> creepList)
        {
            if (creepList.Count > 0 && CreepList != creepList)
            {
                CreepList = creepList;

                for (int i = 0; i < EffectList.Count; i++)
                {
                    EffectList[i].CreepList = creepList;
                }
            }
        }
        
        public void StackReset()
        {
           
            IsStacked = true;
            EffectCount = 0;
            IsOnCooldown = false;

            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i] = Instantiate(EffectList[i]);
                EffectList[i].StackReset();
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
            {
                IsStackable = true;
            }
            else
            {
                IsStackable = false;
            }
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
            if (EffectCount >= EffectList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator NextEffect(float delay)
        {
            yield return new WaitForSeconds(delay);
  

            IsChangingEffect = false;

           

            state.ChangeState(new SetEffectState(this));
        }

        private IEnumerator StartCooldown(float delay)
        {         
            yield return new WaitForSeconds(delay);

            if (IsStackable)
            {
                if (!CheckEffectsEnded())
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
                IsOnCooldown = false;

                if (CheckEffectsEnded())
                {
                    for (int i = 0; i < EffectList.Count; i++)
                    {
                        EffectList[i].ResetEffect();
                    }
                
                    EffectCount = 0;

                    state.ChangeState(new ChoseEffectState(this));
                }
                else
                {
                    state.ChangeState(new SetEffectState(this));
                }
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
                if (!owner.IsOnCooldown)
                {                  
                    if (!owner.CheckIntervalsEnded())
                    {
                        GM.Instance.StartCoroutine(owner.StartCooldown(owner.Cooldown));
                        owner.IsOnCooldown = true;
                        owner.state.ChangeState(new SetEffectState(owner));
                    }
                }
            }

            public void Exit()
            {
               
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
            }

            public void Execute()
            {
                if (!owner.IsChangingEffect && !owner.CheckIntervalsEnded())
                {                   
                    GM.Instance.StartCoroutine(owner.NextEffect(owner.EffectList[owner.EffectCount].NextEffectInterval));
                    owner.IsChangingEffect = true;                  
                }

                if (owner.EffectCount == 0)
                {
                    owner.EffectList[0].InitEffect();
                }
                else
                {
                    for (int i = 0; i < owner.EffectCount; i++)
                    {                     
                        owner.EffectList[i].InitEffect();
                    }
                }
               
                if (owner.CheckEffectsEnded())
                {
                    if (!owner.IsStacked)
                    {
                        owner.state.ChangeState(new ChoseEffectState(owner));
                    }
                }
               
            }

            public void Exit()
            {
                if (!owner.CheckIntervalsEnded())
                {
                    owner.EffectCount++;
                }
            }
        }
        
    }
}