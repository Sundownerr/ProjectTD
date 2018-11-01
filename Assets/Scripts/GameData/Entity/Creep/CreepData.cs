using UnityEngine;

namespace Game.Data.Entity.Creep
{  
    public abstract class CreepData : Entity
    {
        [HideInInspector]
        public float MoveSpeed;

        public float Health, DefaultMoveSpeed;
        public int Exp, Gold;
        public Armor Armor;
        public RaceType Race;

        public GameObject model;
    }
}
