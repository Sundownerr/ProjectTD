
namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Boss", menuName = "Data/Creep/Boss")]

    public class Boss : CreepData
    {
        protected override void Awake()
        {
            base.Awake();
            
            MoveSpeed = DefaultMoveSpeed;
            Exp = 20;
            Gold = 20;
        }
    }
}