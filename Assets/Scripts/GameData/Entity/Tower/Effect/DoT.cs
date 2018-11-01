using System.Collections;
using UnityEngine;
using Game.System;
using System.Collections.Generic;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "DoT", menuName = "Data/Tower/Effect/DoT")]
    public class DoT : Effect
    {
        public int DamagePerTick;
        public GameObject EffectPrefab;

        private float tick;
        private GameObject effectPrefab;
        private ParticleSystem[] psList;    

        public IEnumerator SetEffect(float delay)
        {
            while (tick < Duration)
            {
                tick++;

                if (target != null)
                    target.GetDamage(DamagePerTick, tower);
                else
                {
                    End();
                    break;
                }
                yield return new WaitForSeconds(1f);
            }

            End();
        }

        public override void Start()
        {
            if (target != null)
            {
                effectPrefab = Instantiate(EffectPrefab,
                                target.gameObject.transform.position + Vector3.up * 20,
                                Quaternion.identity,
                                target.gameObject.transform);

                psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                target.creepRenderer.material.color = Color.green;

                Show(true);
            }

            base.Start();

            EffectCoroutine = GM.Instance.StartCoroutine(SetEffect(Duration));
        }

        public override void End()
        {
            if (target != null)
                target.creepRenderer.material.color = Color.white;
    
            Destroy(effectPrefab);
            tick = 0;

            base.End();
        }

        private void Show(bool enabled)
        {
            for (int i = 0; i < psList.Length; i++)
            {
                var emissionModule = psList[i].emission;
                emissionModule.enabled = enabled;

                if (enabled)
                    psList[i].Play();
                else
                    psList[i].Stop();
            }
        }
    }
}
