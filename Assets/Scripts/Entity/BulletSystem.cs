using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS1591 

namespace Game.Tower
{
    public class BulletSystem : ExtendedMonoBehaviour
    {

        public ParticleSystem[] ParticleSystemList;
        public bool IsReachedTarget;
        public float BulletLifetime;
        public GameObject Target;
        public float Speed;
        
        private ParticleSystem.EmissionModule emissionModule;

        protected override void Awake()
        {         
            Speed = 10f;
            Speed = Mathf.Lerp(Speed, Speed * 10, Time.deltaTime * 10f);
            BulletLifetime = ParticleSystemList[0].main.startLifetime.constant;
        }

        private void OnEnable()
        {          
            Show(true);
            IsReachedTarget = false;
        }

        private void OnDisable()
        {
            Show(false);
            IsReachedTarget = true;
        }

        public void Show(bool enabled)
        {
            for (int i = 0; i < ParticleSystemList.Length; i++)
            {
                emissionModule = ParticleSystemList[i].emission;
                emissionModule.enabled = enabled;

                if (enabled)
                {
                    ParticleSystemList[i].Play();
                }
                else
                {
                    ParticleSystemList[i].Stop();
                }
            }
        }      
    }
}