
namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Commander", menuName = "Data/Creep/Commander")]

    public class Commander : CreepData
    {
        protected override void Awake()
        {            
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;

            base.Awake();         
        }
    }
}
