using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    public class Trait : Entity 
    {
        public virtual TraitSystem GetTraitSystem(EntitySystem owner) => new TraitSystem(this, owner);
    }
}
