
namespace Game.Data.Entity.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Normal", menuName = "Creep/Normal")]

    public class Normal : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 2;
        }
    }
}
