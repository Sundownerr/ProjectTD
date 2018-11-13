
using Game.Data;

namespace Game.Systems
{
    public class EntitySystem : ExtendedMonoBehaviour
    {
        public bool IsVulnerable { get => isVulnerable; set => isVulnerable = value; }
        public bool IsOn { get => isOn; set => isOn = value; }
        public EffectSystem EffectSystem { get => effectSystem; set => effectSystem = value; }
        public HealthSystem HealthSystem { get => healthSystem; set => healthSystem = value; }

        protected EffectSystem effectSystem;
        protected HealthSystem healthSystem;
        protected bool isVulnerable, isOn;

        protected override void Awake()
        {
            base.Awake();
            IsOn = true;
        }
    }

}

