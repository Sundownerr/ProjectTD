
namespace Game.Data.Entity.Creep
{
    public class Small : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 1;
        }
    }
}
