using UnityEngine;
using System.Collections.Generic;
using Game.Systems;

namespace Game.Data
{
    public abstract class Effect : Entity
    {
        public bool IsEnded { get => isEnded; set => isEnded = value; }
        public EntitySystem Target { get => target; set => target = value; }

        public float Duration, NextInterval;
        public bool IsStackable;

        protected bool isSet, isEnded;
        private EntitySystem target;
        protected Ability ownerAbility;
        protected Coroutine EffectCoroutine;
        protected List<EntitySystem> AffectedTargetList;

        protected override void SetId() 
        {
            if (Owner is Creep.CreepSystem ownerCreep)
            {
                var stats = ownerCreep.Stats;
                Id.AddRange(stats.Id);  
                Id.Add(stats.AbilityList[stats.AbilityList.IndexOf(ownerAbility)].EffectList.IndexOf(this));         
            }
            else if(Owner is Tower.TowerSystem ownerTower)
            {
                var stats = ownerTower.Stats;
                Id.AddRange(stats.Id);  
                Id.Add(stats.AbilityList[stats.AbilityList.IndexOf(ownerAbility)].EffectList.IndexOf(this));
            }
        }

        public virtual void Apply()
        {
            if (Target == null)
                End();

            isSet = true;
            IsEnded = false;
        }

        public virtual void Continue()
        {
            if (!IsEnded)
                if (Target == null)
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
                RestartState();
            }
            else if (IsEnded)
            {
                RestartState();
            }
        }

        public virtual void RestartState()
        {
            if (AffectedTargetList != null)
                AffectedTargetList.Clear();

            if (EffectCoroutine != null)
                GM.Instance.StopCoroutine(EffectCoroutine);

            End();

            IsEnded = false;
            isSet = false;
        }    

        public virtual void Init()
        {
            if (!isSet)
                Apply();
            
            Continue();
        }

        public virtual void SetTarget(EntitySystem target, bool isForceSet)
        {
            if (isForceSet)
                this.Target = target;
            else
                if (this.Target == null)
                this.Target = target;
        }
    }
}
