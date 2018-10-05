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
        public List<GameObject> creep = new List<GameObject>(), tower = new List<GameObject>();
        public Creep.CreepSystem creepData;
        protected bool isEffectEnded, isEffectSet;
        protected float currentDuration;

        public virtual void InitEffect() {}
    }
}
