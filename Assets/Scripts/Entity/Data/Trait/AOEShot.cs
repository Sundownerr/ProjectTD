using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "AOE Shot", menuName = "Data/Tower/Trait/Multishot")]

    public class AOEShot : Trait, ITrait
    {
        public int Range;

        private void Awake()
        {
            Name = "AOE SHot";
            Description = $"Damage targets in {Range} range";
        }

        public override ITraitSystem GetTraitSystem(EntitySystem owner)
        {
            this.owner = owner; 
            return new AOEShotSystem(this);
        }  
    }
}

