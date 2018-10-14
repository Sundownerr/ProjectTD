using UnityEngine;

namespace Game.Data.Entity
{
    public abstract class Entity : ScriptableObject
    {
        public string EntityName;
        public string EntityDescription;
    }
}