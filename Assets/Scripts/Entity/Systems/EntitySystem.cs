
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
    public abstract class EntitySystem
    {
        public bool IsVulnerable { get => isVulnerable; set => isVulnerable = value; }
        public bool IsOn { get => isOn; set => isOn = value; }
        public EffectSystem EffectSystem { get => effectSystem; set => effectSystem = value; }
        public HealthSystem HealthSystem { get => healthSystem; set => healthSystem = value; }
        public GameObject Prefab { get => prefab; set => prefab = value; }

        protected EffectSystem effectSystem;
        protected HealthSystem healthSystem;
        protected bool isVulnerable, isOn;
        protected GameObject prefab;     
    }
}

