using UnityEngine;
using System.Collections.Generic;
using Game.Systems;

namespace Game
{
    public abstract class Entity : ScriptableObject
    {
        public List<int> Id { get => id; set => id = value; }

        public string Name;
        public string Description;
        
        protected EntitySystem owner; 
        protected List<int> id;

        protected virtual void Awake() 
        {
        }

        public virtual bool CompareId(List<int> otherId)
        {
            if(otherId == null || Id == null)
                return false;

            if(Id.Count != otherId.Count)
                return false;
            else
                for (int i = 0; i < Id.Count; i++)
                    if(Id[i] != otherId[i])
                        return false;

            return true;
        }

        protected virtual void SetName() {}
        protected virtual void SetId() {}

        public virtual void SetOwner(EntitySystem owner) 
        {
            if(owner == null)
                return;
            else
                this.owner = owner;
        }

        public virtual EntitySystem GetOwner() => owner;
        
    }
}