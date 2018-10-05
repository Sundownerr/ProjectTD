using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect, IEffect
    {       
        private float targetMoveSpeed;
       
        public override void InitEffect()
        {            
            if(!isEffectSet)
            {
                targetMoveSpeed = creepData.Stats.moveSpeed;
                GameManager.Instance.StartCoroutine(SetEffect(Duration));
                isEffectEnded = false;
            }

            if (!isEffectEnded)
            {                                                  
                Debug.Log("doing shit");
                creepData.Stats.moveSpeed = 0;
            }
        }

        public IEnumerator SetEffect(float delay)
        {
            isEffectSet = true;

            Debug.Log("before timer");

            yield return new WaitForSeconds(delay);

            Debug.Log("after timer");

            creepData.Stats.moveSpeed = targetMoveSpeed;
            isEffectSet = false;
            isEffectEnded = true;
        }
    }
}
