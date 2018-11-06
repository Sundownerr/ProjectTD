using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Systems;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Data/Ability")]

    [Serializable]
    public class Ability : Entity
    {
        public bool IsNeedStack { get => isNeedStack; set => isNeedStack = value; }
        public bool IsOnCooldown { get => isOnCooldown; set => isOnCooldown = value; }
        public EntitySystem Target { get => target; set => target = value; }
        public bool IsStacked { get => isStacked; set => isStacked = value; }

        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect> EffectList;

        private bool isStackable, isStacked, isNeedStack, isOnCooldown;
        private EntitySystem target;
        private StateMachine state;
        private int effectCount;

        protected override void SetId() 
        {
            if (Owner is Creep.CreepSystem ownerCreep)
            {
                Id.AddRange(ownerCreep.Stats.Id);   
                Id.Add(ownerCreep.Stats.AbilityList.IndexOf(this));           
            }
            else if(Owner is Tower.TowerSystem ownerTower)
            {
                Id.AddRange(ownerTower.Stats.Id);   
                Id.Add(ownerTower.Stats.AbilityList.IndexOf(this));
            }
        }

        public void Init() => state.Update();
        
        public void SetOwner(EntitySystem owner)
        {                 
            Owner = owner;

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].Owner = owner;

            EffectList[EffectList.Count - 1].NextInterval = 0.01f;

            CheckStackable();           

            state = new StateMachine();
            state.ChangeState(new SetEffectState(this));         
        }

        public void SetTarget(EntitySystem target)
        {
            this.Target = target;
            SetEffectsTarget(target);
        }       

        private void SetEffectsTarget(EntitySystem target)
        {
            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].SetTarget(target, EffectList[i].IsStackable);            
        }

        public void StackReset()
        {
            IsStacked = true;

            for (int i = 0; i < EffectList.Count; i++)
                if(EffectList[i].IsStackable)
                    EffectList[i] = Instantiate(EffectList[i]);                    
        }

        public void CooldownReset()
        {
            effectCount = 0;          

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].ApplyRestart();
        }

        private void CheckStackable()
        {
            var allEffectsInterval = 0f;
            var allEffectsDuration = 0f;

            for (int i = 0; i < EffectList.Count; i++)
            {
                allEffectsInterval += EffectList[i].NextInterval;
                allEffectsDuration += EffectList[i].Duration;
            }

            isStackable = 
                allEffectsInterval >= Cooldown ? true : 
                allEffectsDuration >= Cooldown ? true : false;
        }

        public bool CheckAllEffectsEnded()
        {
            for (int i = 0; i < EffectList.Count; i++)
                if (!EffectList[i].IsEnded)
                    return false;        
            return true;
        }

        public bool CheckAllEffectsSet() => effectCount >= EffectList.Count - 1;         
        
        public bool CheckNeedStack()
        {
            if (isStackable)
                for (int i = 0; i < EffectList.Count; i++)
                    if (EffectList[i].IsStackable)
                        if (!EffectList[i].IsEnded)
                            return true;
            return false;         
        }

        private IEnumerator StartCooldown(float delay)
        {
            IsOnCooldown = true;

            yield return new WaitForSeconds(delay);
           
            IsNeedStack = CheckNeedStack();
            CooldownReset();
            IsOnCooldown = false;
        }
     
        protected class SetEffectState : IState
        {
            private readonly Ability o;
            private float timer;

            public SetEffectState(Ability o) => this.o = o; 

            public void Enter() => timer = 0;

            public void Execute()
            {
                timer += Time.deltaTime;

                for (int i = 0; i <= o.effectCount; i++)
                    o.EffectList[i].Init();  

                if(!o.IsStacked && !o.IsOnCooldown)
                    GM.Instance.StartCoroutine(o.StartCooldown(o.Cooldown)); 

                if (!o.CheckAllEffectsSet())
                    if (timer > o.EffectList[o.effectCount].NextInterval)
                    {
                        o.effectCount++;
                        timer = 0;
                    }
            }

            public void Exit() { }
        }        
    }
}