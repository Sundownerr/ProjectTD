
namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Small", menuName = "Data/Creep/Small")]

    public class Small : CreepData
    {
        protected override void Awake()
        {
            base.Awake();
            
            MoveSpeed = DefaultMoveSpeed;
            Exp = 1;
            Gold = 1;
            Type = CreepType.Small;
        }
    }
}
