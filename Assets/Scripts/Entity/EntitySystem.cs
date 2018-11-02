using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class EntitySystem : ExtendedMonoBehaviour
    {
        public bool IsVulnerable;
        protected bool isOn;

        public virtual void ApplyEffect(Data.Effect effect) { }

        public virtual void RemoveEffect(Data.Effect effect) { }

        protected override void Awake()
        {
            base.Awake();
            isOn = true;
        }

        public virtual void SetOn(bool value)
        {
            if (value)
                isOn = true;
            else
                isOn = false;
        }
    }

}

