
namespace Game.Data.Entity.Creep
{
    public class Flying : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
        }
    }
}
