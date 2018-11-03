using UnityEngine;

namespace Game.Tower.Data.Stats
{
    public abstract class Special : ScriptableObject
    {
        public string Name, Description;

        public virtual void IncreaseStatsPerLevel() { }
        public virtual void InitSpecial(TowerSystem ownerTower) { }
    }
}
