using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "AOE Shot", menuName = "Data/Tower/Trait/Multishot")]

    public class AOEShot : Trait
    {
        public int Range;

        private void Awake()
        {
            Name = "AOE SHot";
            Description = $"Damage targets in {Range} range";
        }

        public override TraitSystem GetTraitSystem(EntitySystem owner) => new AOEShotSystem(this, owner);     
    }
}

