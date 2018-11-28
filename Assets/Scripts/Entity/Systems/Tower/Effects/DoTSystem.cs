using System.Collections;
using UnityEngine;
using Game.Systems;
using Game.Creep;
using Game.Tower;
using Game.Data.Effects;
using Game.Data;

namespace Game.Systems.Effects
{   
    public class DoTSystem : EffectSystem
    {       
        private float tickTimer;
        private new DoT effect;
        private GameObject effectPrefab;
        private ParticleSystem[] psList;	

        public DoTSystem(DoT effect, EntitySystem owner) : base(effect, owner)
        {
            this.effect = effect;
        }

        public override void Apply()
        {
            base.Apply();

            if (isMaxStackCount || target == null || target.Prefab == null)
            {
                End();
                return;
            }           
            else
            {                
                effectPrefab = Object.Instantiate(effect.EffectPrefab,
                                target.Prefab.transform.position + Vector3.up * 20,
                                Quaternion.identity,
                                target.Prefab.transform);

                psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                Show(true);     
                
                target.EffectSystem.Add(effect);                            
            }
        }

        public override void Continue()
        {
            base.Continue();
            
            tickTimer += Time.deltaTime;
            if (tickTimer == 1)           
                if (target is CreepSystem creep)
                {
                    tickTimer = 0;
                    DamageSystem.DoDamage(creep,  effect.DamagePerTick, (TowerSystem)owner);
                }
                else            
                    End();              
        }

        public override void End()
        {
            Object.Destroy(effectPrefab);
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
