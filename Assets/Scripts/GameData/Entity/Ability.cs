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

        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect> EffectList;

        private bool isStackable, isStacked, isNeedStack, isOnCooldown;
        private EntitySystem target;
        private StateMachine state;
        private int effectCount;

        private void Awake()
        {
            state = new StateMachine();
            
        }

        public void Init() => state.Update();

        public void SetTarget(EntitySystem target)
        {
            this.target = target;
            SetEffectsTarget(target);
        } 

        private void SetEffectsTarget(EntitySystem target)
        {
            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].SetTarget(target, EffectList[i].IsStackable);            
        }

        public override void SetId() 
        {
            id = new List<int>();

            if (Owner is Creep.CreepSystem ownerCreep)
            {
                id.AddRange(ownerCreep.Stats.Id);   
                id.Add(ownerCreep.Stats.AbilityList.IndexOf(this));           
            }
            else if(Owner is Tower.TowerSystem ownerTower)
            {
                id.AddRange(ownerTower.Stats.Id);   
                id.Add(ownerTower.Stats.AbilityList.IndexOf(this));
            }
        }       
        
        public void SetOwner(EntitySystem owner)
        {                 
            this.owner = owner;
            SetId();

            for (int i = 0; i < EffectList.Count; i++)           
                EffectList[i].SetOwner(owner, this);
            
            EffectList[EffectList.Count - 1].NextInterval = 0.01f;

            CheckStackable();             
            state.ChangeState(new SetEffectState(this));         
        }    

        public void StackReset(EntitySystem owner)
        {
            isStacked = true;

            this.owner = owner;
            SetId();
        
            for (int i = 0; i < EffectList.Count; i++)
                if(EffectList[i].IsStackable)
                {
                    EffectList[i] = Instantiate(EffectList[i]);  
                    EffectList[i].SetOwner(owner, this);
                }                              
                
            state.ChangeState(new SetEffectState(this));    
        }

        public void CooldownReset()
        {
            effectCount = 0;          

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].ApplyRestart();

            isOnCooldown = false;
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
            isOnCooldown = true;

            yield return new WaitForSeconds(delay);
           
            isNeedStack = CheckNeedStack();
            CooldownReset();          
        }
     
        protected class SetEffectState : IState
        {
            private readonly Ability o;
            private float nextEffectTimer;

            public SetEffectState(Ability o) => this.o = o; 

            public void Enter() => nextEffectTimer = 0;

            public void Execute()
            {
                nextEffectTimer += Time.deltaTime;

                for (int i = 0; i <= o.effectCount; i++)
                    o.EffectList[i].Init();  

                if(!o.isStacked && !o.isOnCooldown)
                    GM.Instance.StartCoroutine(o.StartCooldown(o.Cooldown)); 

                if (!o.CheckAllEffectsSet())
                    if (nextEffectTimer > o.EffectList[o.effectCount].NextInterval)
                    {
                        o.effectCount++;
                        nextEffectTimer = 0;
                    }
            }

            public void Exit() { }
        }        
    }
}