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
    public class Effect : Entity
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
        protected float effectTimer;
        protected EntitySystem target;
        protected Ability ownerAbility;

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
            if(IsStackable)
                if(target.EffectSystem.CountOf(this) >= MaxStackCount)
                {
                    isMaxStackCount = true;                
                    return;
                }           
            
            isSet = true;
            IsEnded = false;
        }

        public virtual void Continue()
        {
            if (!isEnded)
            {
                if (target == null)
                    End();     
                       
                effectTimer = effectTimer > Duration ? -1 : effectTimer += Time.deltaTime;

                if(effectTimer == -1)
                    End();
            }             
        }

        public virtual void End() 
        {        
            if(!isMaxStackCount)     
                target?.EffectSystem.Remove(this);

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
            End();
            isMaxStackCount = false;
            IsEnded = false;
            isSet = false;
        }    

        public override void SetId() 
        {
            base.SetId();

            if (Owner is Creep.CreepSystem creep)
            {
                id.AddRange(creep.Stats.Id);  
                id.Add(creep.Stats.AbilityList[creep.Stats.AbilityList.IndexOf(OwnerAbility)].EffectList.IndexOf(this));         
            }
            else if(Owner is Tower.TowerSystem tower)
            {               
                id = tower.Stats.Id;  
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

        public virtual void SetTarget(EntitySystem newTarget) =>     
            Target = Target ?? newTarget;               
    }
}
