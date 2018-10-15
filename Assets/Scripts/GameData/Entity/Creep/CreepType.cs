
namespace Game.Data.Entity.Creep
{  
    public abstract class CreepType : Entity
    {
        public float Health, MoveSpeed, DefaultMoveSpeed;
        public int ArmorIndex, Exp, Gold;
    }
}
