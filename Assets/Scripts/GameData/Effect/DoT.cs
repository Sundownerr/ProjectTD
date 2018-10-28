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

                if (LastCreep != null)
                {
                    LastCreep.GetDamage(DamagePerTick, tower);
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
            if (CreepList.Count > 0)
            {
                AffectedCreepList.Add(CreepList[0]);
                LastCreep = AffectedCreepList[AffectedCreepList.Count - 1];

                if (LastCreep.gameObject != null)
                {
                    effectPrefab = Instantiate(EffectPrefab,
                        LastCreep.gameObject.transform.position + Vector3.up * 20,
                        Quaternion.identity,
                        LastCreep.gameObject.transform);

                    psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                    LastCreep.creepRenderer.material.color = Color.green;

                    Show(true);
                }
                else
                {
                    EndEffect();
                }
            }

            IsSet = true;
            IsEnded = false;

            EffectCoroutine = GM.Instance.StartCoroutine(SetEffect(1f));
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (LastCreep == null)
                {
                    GM.Instance.StopCoroutine(EffectCoroutine);
                    EndEffect();
                }
            }
        }

        public override void EndEffect()
        {
            if (LastCreep != null)
            {
                LastCreep.creepRenderer.material.color = Color.white;
            }

            AffectedCreepList.Remove(LastCreep);
            Destroy(effectPrefab);
            damageTick = 0;
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
