
namespace Game.Data.Entity.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Flying", menuName = "Creep/Flying")]

    public class Flying : CreepType
    {      
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;
        }
    }
}
