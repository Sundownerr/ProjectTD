using UnityEngine;
using System.Collections.Generic;
using Game.Systems;

namespace Game.Data
{
    public abstract class Effect : Entity
    {
        public bool IsEnded { get => isEnded; set => isEnded = value; }
        public EntitySystem Target { get => target; set => target = value; }
        public Ability OwnerAbility { get => ownerAbility; set => ownerAbility = value; }

        public float Duration, NextInterval;
        public bool IsStackable;

        protected bool isSet, isEnded;
        private EntitySystem target;
        private Ability ownerAbility;
        protected Coroutine EffectCoroutine;
        protected List<EntitySystem> AffectedTargetList;

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
            reset = null;
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
