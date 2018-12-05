using Game.Creep;
using Game.Systems;
using UnityEngine;

namespace Game.Tower
{
    public class BulletSystem : EntitySystem
    {
        public EntitySystem Target { get => target; set => target = value; }
        public int RemainingBounceCount { get => remainingBounceCount; set => remainingBounceCount = value >= 0 ? value : 0; }
    
        public float Lifetime { get => lifetime; set => lifetime = value >= 0 ? value : 0; }
        public float Speed { get => speed; set => speed = value >= 0 ? value : 0; }
        public bool IsTargetReached { get => isTargetReached; set => isTargetReached = value; }

        private bool isTargetReached;
        private EntitySystem target;
        private ParticleSystem.EmissionModule emissionModule;
        private ParticleSystem[] particleSystemList;
        private int remainingBounceCount;
        private float lifetime, speed;

        public BulletSystem(GameObject prefab)
        {         
            this.prefab = prefab;
            particleSystemList = prefab.GetComponentsInChildren<ParticleSystem>(true);

            Speed = 10f;
            Speed = Mathf.Lerp(Speed, Speed * 10, Time.deltaTime * 5f);
            Lifetime = particleSystemList[0].main.startLifetime.constant;           
        }

        public void Show(bool enabled)
        {
            for (int i = 0; i < particleSystemList.Length; i++)
            {
                emissionModule = particleSystemList[i].emission;
                emissionModule.enabled = enabled;
               
                if (enabled)
                    particleSystemList[i].Play();
                else
                    particleSystemList[i].Stop();
            }
        }      
    }
}