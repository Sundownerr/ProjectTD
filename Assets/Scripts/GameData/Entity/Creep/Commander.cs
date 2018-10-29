
namespace Game.Data.Entity.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Commander", menuName = "Data/Creep/Commander")]

    public class Commander : CreepData
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;
        }
    }
}
