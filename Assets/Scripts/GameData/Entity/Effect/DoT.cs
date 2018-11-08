using System.Collections;
using UnityEngine;
using Game.Systems;
using Game.Creep;
using Game.Tower;

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

                if (target is CreepSystem creep)
                    creep.GetDamage(DamagePerTick, (TowerSystem)Owner);
                else
                {
                    End();
                    break;
                }
                yield return new WaitForSeconds(1f);
            }

            End();
        }

        public override void Apply()
        {
            base.Apply();

            if(isMaxStackCount || target == null)
            {
                End();
                return;
            }           
            else
            {
                effectPrefab = Instantiate(EffectPrefab,
                                target.gameObject.transform.position + Vector3.up * 20,
                                Quaternion.identity,
                                target.gameObject.transform);

                psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                Show(true);     
                
                target.EffectSystem.ApplyEffect(this);  
                effectCoroutine = GM.Instance.StartCoroutine(SetEffect(Duration));               
            }
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
