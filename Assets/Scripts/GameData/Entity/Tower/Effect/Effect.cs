using UnityEngine;
using System.Collections.Generic;
using Game.System;

namespace Game.Data
{
    public abstract class Effect : Entity
    {
        public string Name, Description;
        public float Duration, NextInterval;
        public bool IsStackable;   

        [HideInInspector]
        public bool IsEnded;

        protected bool IsSet;
        protected EntitySystem target;
        protected Ability ownerAbility;
        protected Coroutine EffectCoroutine;
        protected List<EntitySystem> AffectedTargetList;

        protected new virtual void Awake() => base.Awake();

        protected override void SetId() 
        {
            var tempId = new List<int>();

            if (owner is Creep.CreepSystem ownerCreep)
            {
                tempId.AddRange(ownerCreep.GetStats().Id);              
            }
            else if(owner is Tower.TowerSystem ownerTower)
            {
                var stats = ownerTower.GetStats();
                tempId.AddRange(stats.Id);  
                tempId.Add(stats.AbilityList[stats.AbilityList.IndexOf(ownerAbility)].EffectList.IndexOf(this));
            }

            Id = tempId;
        }

        public virtual void Start()
        {
            if (GetTarget() == null)
                End();

            IsSet = true;
            IsEnded = false;
        }

        public virtual void Continue()
        {
            if (!IsEnded)
                if (GetTarget() == null)
                {
                    End();
                    GM.Instance.StopCoroutine(EffectCoroutine);
                }
        }

        public virtual void End() => IsEnded = true;

        public virtual void ApplyReset()
        {
            if (IsStackable)
            {
                Reset();
            }
            else if (IsEnded)
            {
                Reset();
            }
        }

        public virtual void Reset()
        {
            if (AffectedTargetList != null)
                AffectedTargetList.Clear();

            if (EffectCoroutine != null)
                GM.Instance.StopCoroutine(EffectCoroutine);

            End();

            IsEnded = false;
            IsSet = false;
        }    

        public virtual void Init()
        {
            if (!IsSet)
                Start();
            
            Continue();
        }

        public virtual EntitySystem GetTarget() => target;

        public virtual void SetTarget(EntitySystem target, bool isForceSet)
        {
            if (isForceSet)
                this.target = target;
            else
                if (this.target == null)
                this.target = target;
        }
    }
}
