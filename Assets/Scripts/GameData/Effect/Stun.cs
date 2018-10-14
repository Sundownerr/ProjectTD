using System.Collections;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect
    {
        public GameObject EffectPrefab;

        private GameObject effectPrefab;    

        public override void InitEffect()
        {
            if (!IsSet)
            {
                StartEffect();
            }
            ContinueEffect();
        }

        public IEnumerator SetEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            EndEffect();
        }

        public override void StartEffect()
        {
            if (CreepDataList.Count > 0)
            {
                AffectedCreepData = CreepDataList[0];               

                if (AffectedCreepData != null)
                {
                    effectPrefab = Instantiate(EffectPrefab, AffectedCreepData.transform.position, Quaternion.identity, AffectedCreepData.transform);

                    AffectedCreepData.GetStunned(Duration);
                }
                else
                {
                    EndEffect();
                }

                IsSet = true;
                IsEnded = false;

                GM.Instance.StartCoroutine(SetEffect(Duration));
            }
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (AffectedCreepData == null)
                {
                    GM.Instance.StopCoroutine(SetEffect(Duration));
                    EndEffect();
                }
            }
        }

        public override void EndEffect()
        {
            Destroy(effectPrefab);

           
            IsEnded = true;
        }

        public override void StackReset()
        {
            IsSet = false;
            EndEffect();
            StartEffect();
        }
    }
}
