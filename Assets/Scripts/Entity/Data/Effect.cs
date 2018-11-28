using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using NaughtyAttributes;
using System;
using Game.Creep;
using Game.Tower;

namespace Game.Data
{
    [Serializable]
    public class Effect : Entity
    {     
        public float Duration, NextInterval;
        public bool IsStackable;

        [ShowIf("IsStackable")]
        [MinValue(1), MaxValue(1000)]
        public int MaxStackCount;        

        public virtual EffectSystem GetEffectSystem() => new EffectSystem(this, owner);
    }
}
