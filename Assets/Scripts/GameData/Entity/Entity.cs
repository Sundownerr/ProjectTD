using UnityEngine;

namespace Game
{
    public abstract class Entity : ScriptableObject
    {
        public string EntityName;
        public string EntityDescription;

        [NaughtyAttributes.ReadOnly]
        public int[] Id;

        protected virtual void Awake() 
        {
             if (Id == null || Id.Length == 0)           
                Id = SetId();
        }

        public virtual bool CompareId(int[] otherId)
        {
            if(Id.Length != otherId.Length)
                return false;
            else
                for (int i = 0; i < Id.Length; i++)
                    if(Id[i] != otherId[i])
                        return false;

            return true;
        }

        protected virtual int[] SetId() => null;
    }
}