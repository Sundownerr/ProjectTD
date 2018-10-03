using UnityEngine;
using System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect
    {
        private float currentDuration;
        private readonly float targetMoveSpeed;
       
        private void StunTarget(Creep.CreepSystem targetData)
        {
            if(currentDuration < Duration)
            {
                targetData.Stats.moveSpeed = 0;
                
                currentDuration++;               
            }
            else
            {
                targetData.Stats.moveSpeed = targetMoveSpeed;
            }
        }
    }
}
