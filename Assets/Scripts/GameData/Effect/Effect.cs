using UnityEngine;
using System.Collections.Generic;

namespace Game.Data.Effect
{
    public interface IEffect
    {
        void InitEffect();
        void StartEffect();
        void ContinueEffect();
        void EndEffect();
    }

    public abstract class Effect : ScriptableObject, IEffect
    {
        public string EffectName, EffectDescription;
        public float Duration, NextEffectInterval;
        [HideInInspector]
        public List<Creep.CreepSystem> creepDataList, affectedCreepDataList;
        [HideInInspector]
        public bool isEnded, isSet, isStackable;

        public virtual void ContinueEffect()
        {
           
        }

        public virtual void EndEffect()
        {
            
        }

        public virtual void InitEffect()
        {
          
        }

        public virtual void StartEffect()
        {
           
        }
    }
}
