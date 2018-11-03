using System.Collections;
using UnityEngine;
using Game.Systems;

namespace Game.Data.Effects
{
    [CreateAssetMenu(fileName = "DoT", menuName = "Data/Effect/DoT")]
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

                if (this.target is Creep.CreepSystem target)
                    target.GetDamage(DamagePerTick, (Tower.TowerSystem)owner);
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
            if (this.target is Creep.CreepSystem target)
            {
                effectPrefab = Instantiate(EffectPrefab,
                                target.gameObject.transform.position + Vector3.up * 20,
                                Quaternion.identity,
                                target.gameObject.transform);

                psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();

                Show(true);
            }

            base.Start();

            EffectCoroutine = GM.Instance.StartCoroutine(SetEffect(Duration));
        }

        public override void End()
        {
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
