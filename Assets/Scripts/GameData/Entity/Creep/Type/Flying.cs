
namespace Game.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Flying", menuName = "Data/Creep/Flying")]

    public class Flying : CreepData
    {      
        private void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;
        }
    }
}
