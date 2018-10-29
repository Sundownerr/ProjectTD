using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Data/Tower/Effect/Stun")]
    public class Stun : Effect
    {
        public GameObject EffectPrefab;

        private GameObject effectPrefab;    

        public IEnumerator SetEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            EndEffect();
        }

        public override void StartEffect()
        {
            if (CreepList.Count > 0)
            {
                if (AffectedCreepList == null)
                    AffectedCreepList = new List<Creep.CreepSystem>();

                AffectedCreepList.Add(CreepList[0]);

                LastCreep = AffectedCreepList[AffectedCreepList.Count - 1];

                if (LastCreep.gameObject != null)
                {
                    effectPrefab = Instantiate(EffectPrefab, LastCreep.gameObject.transform.position, Quaternion.identity, LastCreep.gameObject.transform);

                    LastCreep.GetStunned(Duration);
                }
                else
                    EndEffect();
                

                IsSet = true;
                IsEnded = false;

                EffectCoroutine = GM.Instance.StartCoroutine(SetEffect(Duration));
            }

        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
                if (LastCreep == null)
                {
                    EndEffect();
                    GM.Instance.StopCoroutine(EffectCoroutine);
                }
        }

        public override void EndEffect()
        {           
            Destroy(effectPrefab);
            AffectedCreepList.Remove(LastCreep);
            IsEnded = true;
        }
    }
}
