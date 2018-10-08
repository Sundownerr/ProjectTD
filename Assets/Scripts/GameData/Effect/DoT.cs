using System.Collections;
using System.Collections.Generic;
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

                if (affectedCreepData != null)
                {
                    affectedCreepData.GetDamage(DamagePerTick);
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
            if (creepDataList.Count > 0)
            {
                affectedCreepData = creepDataList[0];

                if (affectedCreepData != null)
                {
                    effectPrefab = Instantiate(EffectPrefab,
                        affectedCreepData.transform.position + Vector3.up * affectedCreepData.transform.GetChild(0).lossyScale.x / 2,
                        Quaternion.identity, 
                        affectedCreepData.transform);

                    psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                    affectedCreepData.creepRenderer.material.color = Color.green;
                    Show(true);
                }
                else
                {
                    EndEffect();
                }
            }

            IsSet = true;
            IsEnded = false;

            GameManager.Instance.StartCoroutine(SetEffect(1f));
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (affectedCreepData == null)
                {
                    GameManager.Instance.StopCoroutine(SetEffect(1f));
                    EndEffect();
                }
            }
        }

        public override void EndEffect()
        {
            if (affectedCreepData != null)
            {
                affectedCreepData.creepRenderer.material.color = Color.white;

                Destroy(effectPrefab);
            }

            damageTick = 0;

            IsSet = false;
            IsEnded = true;
        }      
    }
}
