using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Creep
{
    [CreateAssetMenu(fileName = "Creep", menuName = "Base Creep")]
    public class CreepStats : Entity
    {
        public float Health, MoveSpeed, DefaultMoveSpeed;
        public int ArmorIndex;

        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
        }
    }


}
