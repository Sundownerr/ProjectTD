
namespace Game.Data.Entity.Creep
{
    public class Normal : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 2;
        }
    }
}
