using UnityEngine;
using System.Collections.Generic;

namespace Game.Data.Effect
{
    
    public abstract class Effect : ScriptableObject
    {
        public string EffectName, EffectDescription;
        public float Duration, NextEffectInterval;

        [HideInInspector]
        public List<Creep.CreepSystem> CreepDataList;

        [HideInInspector]
        public Creep.CreepSystem AffectedCreepData;

        [HideInInspector]
        public bool IsEnded;

        protected bool IsSet;

        public virtual void InitEffect() { }
        public virtual void StartEffect() { }
        public virtual void ContinueEffect() { }
        public virtual void EndEffect() { }
    }
}
