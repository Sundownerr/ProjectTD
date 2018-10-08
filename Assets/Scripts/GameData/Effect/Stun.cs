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

                GameManager.Instance.StartCoroutine(SetEffect(Duration));
            }
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (AffectedCreepData == null)
                {
                    GameManager.Instance.StopCoroutine(SetEffect(Duration));
                    EndEffect();
                }
            }
        }

        public override void EndEffect()
        {
            Destroy(effectPrefab);

            IsSet = false;
            IsEnded = true;
        }

        public override void StackReset()
        {
            EndEffect();
            StartEffect();
        }
    }
}
