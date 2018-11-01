using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creep.Data
{
    public class Ability : ScriptableObject
    {
        public string AbilityName, AbilityDescription;

        protected CreepSystem ownerCreep;
        protected bool isSet;

        public virtual void Init() { }

        public virtual void SetOwnerCreep(CreepSystem creep)
        {
            ownerCreep = creep;
        }

        public virtual void Start() { }
        public virtual void Continue() { }
        public virtual void End() { }
    }
}