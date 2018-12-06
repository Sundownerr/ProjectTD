using System.Collections.Generic;
using Game.Data;

namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Boss", menuName = "Data/Creep/Boss")]

    public class Boss : CreepData
    {
        public List<Ability> Abilities { get => abilities; set => abilities = value; }
        
        private List<Ability> abilities;

        protected override void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 20;
            Gold = 20;

            base.Awake();                
        }
    }
}