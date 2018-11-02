
namespace Game.Creep
{
    [UnityEngine.CreateAssetMenu(fileName = "Normal", menuName = "Data/Creep/Normal")]

    public class Normal : CreepData
    {
        protected override void Awake()
        {
            base.Awake();
            
            MoveSpeed = DefaultMoveSpeed;
            Exp = 2;
            Gold = 2;
        }
    }
}
