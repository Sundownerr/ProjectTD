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
        
        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect> EffectList;

        private bool isStackable, isStacked, isNeedStack, isOnCooldown;
        private EntitySystem target;
        private StateMachine state;
        private int effectCount;
        private float timer;

        protected override void Awake()
        {
            base.Awake();   
        }

        protected override void SetId() 
        {
            var tempId = new List<int>();

            if (owner is Creep.CreepSystem ownerCreep)
            {
                tempId.AddRange(ownerCreep.Stats.Id);   
                tempId.Add(ownerCreep.Stats.AbilityList.IndexOf(this));           
            }
            else if(owner is Tower.TowerSystem ownerTower)
            {
                tempId.AddRange(ownerTower.Stats.Id);   
                tempId.Add(ownerTower.Stats.AbilityList.IndexOf(this));
            }

            Id = tempId;
        }

        public void Init() => state.Update();

        public EntitySystem GetTarget() => target;
        
        public override void SetOwner(EntitySystem owner)
        {           
            base.SetOwner(owner);

            if(owner == null)
                return;
            else
            {
                for (int i = 0; i < EffectList.Count; i++)
                    EffectList[i].SetOwner(owner);

                EffectList[EffectList.Count - 1].NextInterval = 0.01f;

                CheckStackable();           

                state = new StateMachine();
                state.ChangeState(new SetEffectState(this));
            }
        }

        public void SetTarget(EntitySystem target)
        {
            this.target = target;
            SetEffectsTarget(target);
        }       

        private void SetEffectsTarget(EntitySystem target)
        {
            for (int i = 0; i < EffectList.Count; i++)
                if(EffectList[i].IsStackable)
                    EffectList[i].SetTarget(target, true);
                else 
                    EffectList[i].SetTarget(target, false);
        }

        public void StackReset()
        {
            isStacked = true;

            for (int i = 0; i < EffectList.Count; i++)
                if(EffectList[i].IsStackable)
                    EffectList[i] = Instantiate(EffectList[i]);                    
        }

        public void CooldownReset()
        {
            timer = 0;
            effectCount = 0;          

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].ApplyReset();
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

        public bool CheckAllEffectsSet() => 
            effectCount >= EffectList.Count - 1 ? true : false;         
        
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

            public SetEffectState(Ability o) => this.o = o; 

            public void Enter() { }

            public void Execute()
            {
                o.timer += Time.deltaTime;

                for (int i = 0; i <= o.effectCount; i++)
                    o.EffectList[i].Init();  

                if(!o.isStacked)
                    if(!o.IsOnCooldown)
                        GM.Instance.StartCoroutine(o.StartCooldown(o.Cooldown)); 

                if (!o.CheckAllEffectsSet())
                    if (o.timer > o.EffectList[o.effectCount].NextInterval)
                    {
                        o.effectCount++;
                        o.timer = 0;
                    }
            }

            public void Exit() { }
        }        
    }
}