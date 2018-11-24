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
        
        private GameObject effectPrefab;
        private ParticleSystem[] psList;
        private float tickTimer;

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
                                target.Prefab.transform.position + Vector3.up * 20,
                                Quaternion.identity,
                                target.Prefab.transform);

                psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                Show(true);     
                
                target.EffectSystem.Add(this);                            
            }
        }

        public override void Continue()
        {
            base.Continue();
            
            tickTimer += Time.deltaTime;
            if(tickTimer == 1)           
                if (target is CreepSystem creep)
                {
                    tickTimer = 0;
                    DamageSystem.DoDamage(creep, DamagePerTick, (TowerSystem)Owner);
                }
                else            
                    End();              
        }

        public override void End()
        {
            Destroy(effectPrefab);
            tickTimer = 0;   
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
