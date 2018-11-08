using UnityEngine;
using System.Collections.Generic;
using Game.Systems;
using NaughtyAttributes;
using System;
using Game.Creep;
using Game.Tower;

namespace Game.Data
{
    [Serializable]
    public abstract class Effect : Entity
    {
        public bool IsEnded { get => isEnded; set => isEnded = value; }
        public EntitySystem Target { get => target; set => target = value; }
        public Ability OwnerAbility { get => ownerAbility; set => ownerAbility = value; }

        public float Duration, NextInterval;
        public bool IsStackable;

        [ShowIf("IsStackable")]
        [MinValue(1), MaxValue(1000)]
        public int MaxStackCount;

        protected bool isSet, isEnded, isMaxStackCount;
        protected EntitySystem target;
        private Ability ownerAbility;
        protected Coroutine EffectCoroutine;

        private void Awake()
        {
            if(!IsStackable)
                MaxStackCount = 1;
        }

        public virtual void Init()
        {
            if (!isSet)
                Apply();
            
            Continue();
        }

        public virtual void Apply()
        {
            if (target == null)
            {
                End();   
                return;
            }
    
            if(target.EffectSystem.CountOf(this) >= MaxStackCount)
            {
                isMaxStackCount = true;
                EndMaxStack();
                return;
            }           
           
            isSet = true;
            IsEnded = false;
        }

        public virtual void Continue()
        {
            if (!isEnded)
                if (target == null)
                {
                    End();
                    GM.Instance.StopCoroutine(EffectCoroutine);
                }
        }

        public virtual void End() 
        {             
            if(target != null)
                if(!isMaxStackCount)              
                    if(target.EffectSystem.CountOf(this) > 0)
                        target.EffectSystem.RemoveEffect(this);

            IsEnded = true;
        } 

        public virtual void EndMaxStack()
        {       
            if(target != null)
                if(!isMaxStackCount)              
                    if(target.EffectSystem.CountOf(this) > MaxStackCount)
                        target.EffectSystem.RemoveEffect(this);

            IsEnded = true;
        }
        
        public virtual void ApplyRestart()
        {
            if(IsStackable)
                RestartState();
            else if(isEnded)
                RestartState();     
        }

        public virtual void RestartState()
        {
            if (EffectCoroutine != null)
                GM.Instance.StopCoroutine(EffectCoroutine);

            
            End();
            isMaxStackCount = false;
            IsEnded = false;
            isSet = false;
        }    

        public override void SetId() 
        {
            id = new List<int>();

            if (Owner is Creep.CreepSystem creep)
            {
                id.AddRange(creep.Stats.Id);  
                id.Add(creep.Stats.AbilityList[creep.Stats.AbilityList.IndexOf(OwnerAbility)].EffectList.IndexOf(this));         
            }
            else if(Owner is Tower.TowerSystem tower)
            {               
                id.AddRange(tower.Stats.Id);  
                id.Add(ownerAbility.EffectList.IndexOf(this));
            }
        }

        public void SetOwner(EntitySystem owner, Ability ownerAbility)
        {
            this.owner = owner;
            this.ownerAbility = ownerAbility;
            SetId();
            MaxStackCount = MaxStackCount >= 1 ? MaxStackCount : 1;
        }

        public virtual void SetTarget(EntitySystem newTarget, bool isEffectStackable) =>     
            Target = isEffectStackable ? newTarget : Target ?? newTarget;               
    }
}
