
namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Normal", menuName = "Data/Creep/Normal")]

    public class Normal : CreepData
    {
        protected override void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 2;
            Gold = 2;

            base.Awake();                  
        }
    }
}
