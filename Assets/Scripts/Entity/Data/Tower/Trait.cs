using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    public abstract class Trait : ScriptableObject
    {
        public string Name, Description;

        public virtual void IncreaseStatsPerLevel() { }
        public virtual void InitTrait(TowerSystem ownerTower) { }
    }
}
