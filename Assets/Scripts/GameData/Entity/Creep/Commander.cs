
namespace Game.Data.Entity.Creep
{
    public class Commander : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
        }
    }
}
