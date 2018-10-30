using UnityEngine;
using System.Collections.Generic;
using Game.System;

namespace Game.Data.Effect
{   
    public abstract class Effect : ScriptableObject
    {
        public string EffectName, EffectDescription;
        public float Duration, NextEffectInterval;

        [HideInInspector]
        public List<Creep.CreepSystem> TargetList;
        public Creep.CreepSystem Target;

        [HideInInspector]
        public bool IsEnded;

        [HideInInspector]
        public Tower.TowerBaseSystem tower;

        protected bool IsSet;
        
        protected Coroutine EffectCoroutine;
        protected List<Creep.CreepSystem> AffectedCreepList;
        
        public virtual void Start()
        {
            if (Target == null)
                End();

            IsSet = true;
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

        public virtual void End()
        {
            Target = null;
            IsEnded = true;
        }

        public virtual void Reset()
        {          
            if (AffectedCreepList != null)
                AffectedCreepList.Clear();

            if (EffectCoroutine != null)
                GM.Instance.StopCoroutine(EffectCoroutine);

            End();

            IsEnded = false;
            IsSet = false;
        }

        public virtual void StackReset() { }

        public virtual void Init()
        {
            if (!IsSet)
                Start();
            
            Continue();
        }

        public virtual void SetTarget(Creep.CreepSystem target)
        {
            if (Target == null)
                Target = target;
        }
    }
}
