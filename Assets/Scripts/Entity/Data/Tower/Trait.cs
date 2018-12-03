using UnityEngine;

namespace Game.Tower.Data.Stats
{
    public abstract class Trait : ScriptableObject
    {
        public string Name, Description;

        public virtual void IncreaseStatsPerLevel() { }
        public virtual void InitTrait(TowerSystem ownerTower) { }
    }
}
