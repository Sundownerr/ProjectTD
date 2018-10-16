
namespace Game.Data.Entity.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Small", menuName = "Creep/Small")]

    public class Small : CreepData
    {
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 1;
            Gold = 1;
        }
    }
}
