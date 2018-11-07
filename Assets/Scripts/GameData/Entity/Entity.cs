using UnityEngine;
using System.Collections.Generic;
using Game.Systems;

namespace Game
{
    public abstract class Entity : ScriptableObject
    {
        public List<int> Id { get => id; set => id = value; }
        public EntitySystem Owner { get => owner; set => owner = value; }

        public string Name;
        public string Description;

        protected EntitySystem owner;
        protected List<int> id;     

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
        public virtual void SetId() {}

        public virtual string GetId()
        {
            var stringBuilder = new System.Text.StringBuilder();

            for (int i = 0; i < id.Count; i++)            
                stringBuilder.Append(id[i].ToString());
           
            return stringBuilder.ToString();
        }
    }
}