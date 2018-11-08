
namespace Game.Creep.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "Flying", menuName = "Data/Creep/Flying")]

    public class Flying : CreepData
    {      
        protected override void Awake()
        {
            MoveSpeed = DefaultMoveSpeed;
            Exp = 4;
            Gold = 4;
            type = CreepType.Flying;

            base.Awake();                  
        }
    }
}
