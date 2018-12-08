using UnityEngine;
using System.Collections.Generic;
using Game.Systems;
using System.Text;

namespace Game
{
    
    public abstract class Entity : ScriptableObject
    {     
        public List<int> Id { get => id; set => id = value; }
        public EntitySystem Owner { get => owner; set => owner = value; }

        public string Name;
        public string Description;

        protected EntitySystem owner;
        [SerializeField]
        protected List<int> id;     

        public virtual bool CompareId(List<int> otherId)
        {
            if (otherId == null || id == null)
                return false;

            if (id.Count != otherId.Count)
                return false;
            
            for (int i = 0; i < id.Count; i++)
                if (id[i] != otherId[i])
                    return false;

            return true;
        }

        protected virtual void SetName() {}

        public virtual void SetId() 
        {
            id = new List<int>(); 
        }

        public virtual string GetId()
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < id.Count; i++)            
                stringBuilder.Append(id[i].ToString());
           
            return stringBuilder.ToString();
        }
    }
}