using UnityEngine;

namespace Game.Data.Entity.Creep
{  
    public abstract class CreepData : Entity
    {
        public float Health, MoveSpeed, DefaultMoveSpeed;
        public int RaceId, Exp, Gold;
        public Armor Armor;

        public GameObject model;
    }
}
