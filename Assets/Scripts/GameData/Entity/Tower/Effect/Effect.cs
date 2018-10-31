using UnityEngine;
using System.Collections.Generic;
using Game.System;

namespace Game.Data.Effect
{
    public abstract class Effect : ScriptableObject
    {
        public string EffectName, EffectDescription;
        public float Duration, NextInterval;
        public bool IsStackable;

        [HideInInspector]
        public float LeftDuration;

        [HideInInspector]
        public List<Creep.CreepSystem> TargetList;           

        [HideInInspector]
        public bool IsEnded;

        [HideInInspector]
        public Tower.TowerBaseSystem tower;

        protected bool IsSet;
        protected Creep.CreepSystem target;
        protected Coroutine EffectCoroutine;
        protected List<Creep.CreepSystem> AffectedCreepList;

        public virtual void Start()
        {
            if (GetTarget() == null)
                End();

            LeftDuration = Duration;
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
                else if (LeftDuration < Duration)
                    LeftDuration -= Time.deltaTime;
        }

        public virtual void End()
        {
            IsEnded = true;
        }

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
            if (AffectedCreepList != null)
                AffectedCreepList.Clear();

            if (EffectCoroutine != null)
                GM.Instance.StopCoroutine(EffectCoroutine);

            End();

            IsEnded = false;
            IsSet = false;
        }    

        public virtual void StackReset(float leftDuration)
        {
        }

        public virtual void Init()
        {
            if (!IsSet)
                Start();
            
            Continue();
        }

        public virtual Creep.CreepSystem GetTarget()
        {
            return target;
        }

        public virtual void SetTarget(Creep.CreepSystem target)
        {          
            this.target = target;
        }
    }
}
