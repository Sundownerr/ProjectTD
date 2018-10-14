using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Creep
{  
    public abstract class CreepType : Entity
    {
        public float Health, MoveSpeed, DefaultMoveSpeed;
        public int ArmorIndex, Exp;
    }
}
