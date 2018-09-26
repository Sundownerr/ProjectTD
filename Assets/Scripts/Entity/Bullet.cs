using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{
    public class Bullet : ExtendedMonoBehaviour
    {
        public ParticleSystem[] particleSystemList;
        public float speed;
        public bool isDestinationReached;
        public GameObject Target;
        private ParticleSystem.EmissionModule emissionModule;

        protected override void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            isDestinationReached = false;
            
            for (int i = 0; i < particleSystemList.Length; i++)
            {
                emissionModule = particleSystemList[i].emission;
                emissionModule.enabled = true;
                particleSystemList[i].Play();
            }
        }

        public void DisableParticles()
        {
            for (int i = 0; i < particleSystemList.Length; i++)
            {
                emissionModule = particleSystemList[i].emission;
                emissionModule.enabled = false;
                particleSystemList[i].Stop();
            }

          
        }
    }
}