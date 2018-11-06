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

        private delegate void resetAction();  
        public virtual void ApplyRestart()
        {
            resetAction reset = IsStackable ? RestartState : isEnded ? RestartState : (resetAction)null;
            reset?.Invoke();          
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

        public virtual void SetTarget(EntitySystem newTarget, bool isEffectStackable) =>     
            Target = isEffectStackable ? newTarget : Target ?? newTarget;               
    }
}
