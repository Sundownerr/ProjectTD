
namespace Game.Data.Entity.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Boss", menuName = "Data/Creep/Boss")]

    public class Boss : CreepData
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 20;
            Gold = 20;
        }
    }
}