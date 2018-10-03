using UnityEngine;
using System;

namespace Game.Data.Effect
{
   
    public class Stun : Effect
    {
        private float currentDuration;
        private readonly float targetMoveSpeed;
        private Creep.CreepSystem targetData;

        public Stun(GameObject target, float duration)
        {
            this.Duration = duration;
            currentDuration = 0;

            targetData = target.GetComponent<Creep.CreepSystem>();

            targetMoveSpeed = targetData.Stats.moveSpeed;

            StunTarget();
        }

       
        private void StunTarget()
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
