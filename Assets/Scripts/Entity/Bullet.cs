using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class Bullet : ExtendedMonoBehaviour
    {
        public ParticleSystem[] ParticleSystemList;
        public float Speed;
        public bool IsDestinationReached;
        public float BulletLifetime;
        public GameObject Target;
        private ParticleSystem.EmissionModule emissionModule;

        protected override void Awake()
        {
            gameObject.SetActive(false);
            Speed = 10f;
            Speed = Mathf.Lerp(Speed, Speed * 10, Time.deltaTime * 10f);
            BulletLifetime = ParticleSystemList[0].main.startLifetime.constant;
        }

        private void OnEnable()
        {
            IsDestinationReached = false;
            
            for (int i = 0; i < ParticleSystemList.Length; i++)
            {
                emissionModule = ParticleSystemList[i].emission;
                emissionModule.enabled = true;
                ParticleSystemList[i].Play();
            }
        }

        public void DisableParticles()
        {
            for (int i = 0; i < ParticleSystemList.Length; i++)
            {
                emissionModule = ParticleSystemList[i].emission;
                emissionModule.enabled = false;
                ParticleSystemList[i].Stop();
            }
        }
    }
}