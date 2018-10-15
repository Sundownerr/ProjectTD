
namespace Game.Data.Entity.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Commander", menuName = "Creep/Commander")]

    public class Commander : CreepType
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;
        }
    }
}
