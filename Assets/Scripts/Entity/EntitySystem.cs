
namespace Game.Systems
{
    public class EntitySystem : ExtendedMonoBehaviour
    {
        public bool IsVulnerable { get => isVulnerable; set => isVulnerable = value; }
        public bool IsOn { get => isOn; set => isOn = value; }

        protected bool isVulnerable, isOn;

        public virtual void ApplyEffect(Data.Effect effect) { }

        public virtual void RemoveEffect(Data.Effect effect) { }

        protected override void Awake()
        {
            base.Awake();
            IsOn = true;
        }
    }

}

