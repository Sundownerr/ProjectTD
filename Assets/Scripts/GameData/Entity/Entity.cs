using UnityEngine;
using System.Collections.Generic;
using Game.System;

namespace Game
{
    public abstract class Entity : ScriptableObject
    {
        public string EntityName;
        public string EntityDescription;

        protected EntitySystem owner;

        [NaughtyAttributes.ReadOnly]
        public List<int> Id;

        protected virtual void Awake() 
        {
             if (Id == null || Id.Count == 0)           
                SetId();
        }

        public virtual bool CompareId(List<int> otherId)
        {
            if(Id.Count != otherId.Count)
                return false;
            else
                for (int i = 0; i < Id.Count; i++)
                    if(Id[i] != otherId[i])
                        return false;

            return true;
        }

        protected virtual void SetId() {}

        public virtual void SetOwner(EntitySystem owner) => this.owner = owner;

        public virtual EntitySystem GetOwner() => owner;
        
    }
}