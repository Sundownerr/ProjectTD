using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Multishot", menuName = "Data/Tower/Trait/Multishot")]

    public class Multishot : Trait
    {
        public int Count;

        private void Awake()
        {
            Name = "Multishot";
            Description = $"Shoot {Count} additional targets";
        }

        public override TraitSystem GetTraitSystem(EntitySystem owner) => new MultishotSystem(this, owner);     
    }
}
