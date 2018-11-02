
namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Flying", menuName = "Data/Creep/Flying")]

    public class Flying : CreepData
    {      
        protected override void Awake()
        {
            base.Awake();
            
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;
        }
    }
}
