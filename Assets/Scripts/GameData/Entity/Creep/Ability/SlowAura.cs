using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Creep.Data.Abilities
{
    public class SlowAura : Ability
    {
        public float Cooldown;

        private IEnumerator SetEffect(float delay)
        {
            isSet = true;
            Start();

            yield return new WaitForSeconds(delay);

            isSet = false;
        }

        public override void Init()
        {
            if(!isSet)
            {
                if (ownerCreep.GetDamageDealer() != null)
                {
                    GM.Instance.StartCoroutine(SetEffect(Cooldown));
                }
            }
            
        }

        public override void Start()
        {
           
        }
    }
}