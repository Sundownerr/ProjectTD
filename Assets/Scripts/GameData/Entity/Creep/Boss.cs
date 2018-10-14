
namespace Game.Data.Entity.Creep
{
    public class Boss : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 20;
        }
    }
}