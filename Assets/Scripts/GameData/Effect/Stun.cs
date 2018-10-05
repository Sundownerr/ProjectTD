using UnityEngine;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect, IEffect
    {       
        private readonly float targetMoveSpeed;
       
        public override void InitEffect()
        {
            Debug.Log("init stun");
            if (currentDuration < Duration)
            {
                creepData.Stats.moveSpeed = 0;
                
                currentDuration++;               
            }
            else
            {
                creepData.Stats.moveSpeed = targetMoveSpeed;
            }
        }
    }
}
