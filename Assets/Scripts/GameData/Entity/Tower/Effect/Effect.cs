using UnityEngine;
using System.Collections.Generic;

namespace Game.Data.Effect
{
    
    public abstract class Effect : ScriptableObject
    {
        public string EffectName, EffectDescription;
        public float Duration, NextEffectInterval;

        [HideInInspector]
        public List<Creep.CreepSystem> CreepList;

        [HideInInspector]
        public List<Creep.CreepSystem> AffectedCreepList;   

        [HideInInspector]
        public bool IsEnded, IsLastInList;

        [HideInInspector]
        public Tower.TowerBaseSystem tower;

        protected bool IsSet;
        protected Creep.CreepSystem LastCreep;
        protected Coroutine EffectCoroutine;

        public virtual void InitEffect() { }
        public virtual void StartEffect() { }
        public virtual void ContinueEffect() { }
        public virtual void EndEffect() { }
        public virtual void StackReset() { }

        public virtual void ResetEffect()
        {
            IsEnded = false;
            IsSet = false;
        }
    }
}
