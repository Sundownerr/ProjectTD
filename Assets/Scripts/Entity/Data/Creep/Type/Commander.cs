using System.Collections.Generic;
using Game.Data;

namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Commander", menuName = "Data/Creep/Commander")]

    public class Commander : CreepData
    {
        public List<Ability> Abilities { get => abilities; set => abilities = value; }

        private List<Ability> abilities;

        protected override void Awake()
        {            
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;

            base.Awake();         
        }
    }
}
