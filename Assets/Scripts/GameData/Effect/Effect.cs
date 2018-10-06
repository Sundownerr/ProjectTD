using UnityEngine;
using System.Collections.Generic;

namespace Game.Data.Effect
{
    public interface IEffect
    {
        void InitEffect();
    }

    public class Effect : ScriptableObject, IEffect
    {
        public string EffectName = "effectname", EffectDescription = "effectname";
        public float Duration = 0, NextEffectInterval;
        public List<Creep.CreepSystem> creepDataList, affectedCreepDataList;
        public bool isEnded, isSet, isStackable;
        protected float currentDuration;

        public virtual void InitEffect() {}
    }
}
