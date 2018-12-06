using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    public interface ITrait
    {
        ITraitSystem GetTraitSystem(EntitySystem owner);
    }

    public class Trait : Entity, ITrait
    {
        public virtual ITraitSystem GetTraitSystem(EntitySystem owner) => null;
    }
}
