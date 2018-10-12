using System.Collections;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "DoT", menuName = "Base DoT")]
    public class DoT : Effect
    {
        public int DamagePerTick;
        public GameObject EffectPrefab;

        private float damageTick;
        private GameObject effectPrefab;
        private ParticleSystem[] psList;

      

        private void Show(bool enabled)
        {
            for (int i = 0; i < psList.Length; i++)
            {
                var emissionModule = psList[i].emission;
                emissionModule.enabled = enabled;

                if (enabled)
                {
                    psList[i].Play();

                }
                else
                {
                    psList[i].Stop();
                }
            }
        }

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
            while (damageTick < Duration)
            {
                damageTick++;

                if (AffectedCreepData != null)
                {
                    AffectedCreepData.GetDamage(DamagePerTick);
                }
                else
                {
                    EndEffect();
                    break;
                }
                yield return new WaitForSeconds(delay);
            }
            EndEffect();
        }

        public override void StartEffect()
        {
            if (CreepDataList.Count > 0)
            {
                AffectedCreepData = CreepDataList[0];

                if (AffectedCreepData != null)
                {
                    effectPrefab = Instantiate(EffectPrefab,
                        AffectedCreepData.transform.position + Vector3.up * AffectedCreepData.transform.GetChild(0).lossyScale.y / 2,
                        Quaternion.identity, 
                        AffectedCreepData.transform);

                    psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                    AffectedCreepData.creepRenderer.material.color = Color.green;
                    Show(true);
                }
                else
                {
                    EndEffect();
                }
            }

            IsSet = true;
            IsEnded = false;

            GM.Instance.StartCoroutine(SetEffect(1f));
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (AffectedCreepData == null)
                {
                    GM.Instance.StopCoroutine(SetEffect(1f));
                    EndEffect();
                }
            }
        }

        public override void EndEffect()
        {
            if (AffectedCreepData != null)
            {
                AffectedCreepData.creepRenderer.material.color = Color.white;

                Destroy(effectPrefab);
            }

            damageTick = 0;

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
