using UnityEngine;

namespace Game.Data.Entity.Creep
{  
    public abstract class CreepData : Entity
    {
        public float Health, MoveSpeed, DefaultMoveSpeed;
        public int ArmorId, RaceId, Exp, Gold;

        public GameObject model;
    }
}
