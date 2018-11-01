using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Creep
{
    public class Ability : ScriptableObject
    {
        public string AbilityName, AbilityDescription;

        protected Game.Creep.CreepSystem ownerCreep;
        protected bool isSet;

        public virtual void Init() { }

        public virtual void SetOwnerCreep(Game.Creep.CreepSystem creep)
        {
            ownerCreep = creep;
        }

        public virtual void Start() { }
        public virtual void Continue() { }
        public virtual void End() { }
    }
}