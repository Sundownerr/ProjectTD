using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Multishot", menuName = "Data/Tower/Trait/Multishot")]

    public class Multishot : Trait, ITrait
    {
        public int Count;

        private void Awake()
        {
            Name = "Multishot";
            Description = $"Shoot {Count} additional targets";
        }

        public override ITraitSystem GetTraitSystem(EntitySystem owner)
        {
            this.owner = owner; 
            return new MultishotSystem(this);
        }  
    }
}
