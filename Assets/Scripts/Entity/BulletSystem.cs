using UnityEngine;

namespace Game.Tower
{
    public class BulletSystem : ExtendedMonoBehaviour
    {
        public ParticleSystem[] ParticleSystemList;
        public bool IsReachedTarget;
        public float Lifetime, Speed;
        public int chainCount;
        public GameObject Target;
                        
        private ParticleSystem.EmissionModule emissionModule;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            Speed = 10f;
            Speed = Mathf.Lerp(Speed, Speed * 10, Time.deltaTime * 10f);
            Lifetime = ParticleSystemList[0].main.startLifetime.constant;
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