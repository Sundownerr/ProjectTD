using Game.Creep;
using Game.Systems;
using UnityEngine;

namespace Game.Tower
{
    public class BulletSystem : ExtendedMonoBehaviour
    {
        public EntitySystem Target { get => target; set => target = value; }
        public int RemainingBounceCount { get => remainingBounceCount; set => remainingBounceCount = value >= 0 ? value : 0; }
        public int ChainshotCount { get => chainshotCount; set => chainshotCount = value >= 0 ? value : 0; }
        public int MultishotCount { get => multishotCount; set => multishotCount = value >= 0 ? value : 0; }
        public int AOEShotRange { get => aOEShotRange; set => aOEShotRange = value >= 0 ? value : 0; }
        public float Lifetime { get => lifetime; set => lifetime = value >= 0 ? value : 0; }
        public float Speed { get => speed; set => speed = value >= 0 ? value : 0; }
        public bool IsTargetReached { get => isTargetReached; set => isTargetReached = value; }

        private bool isTargetReached;
        private EntitySystem target;
        private ParticleSystem.EmissionModule emissionModule;
        private ParticleSystem[] particleSystemList;
        private int remainingBounceCount, chainshotCount, multishotCount, aOEShotRange;
        private float lifetime, speed;

        protected override void Awake()
        {
            base.Awake();

            particleSystemList = GetComponentsInChildren<ParticleSystem>(true);

            Speed = 10f;
            Speed = Mathf.Lerp(Speed, Speed * 10, Time.deltaTime * 5f);
            Lifetime = particleSystemList[0].main.startLifetime.constant;           
        }

        private void OnEnable()
        {          
            Show(true);
            IsTargetReached = false;
            RemainingBounceCount = ChainshotCount > 0 ? ChainshotCount : 0;   
        }

        private void OnDisable()
        {
            Show(false);                         
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