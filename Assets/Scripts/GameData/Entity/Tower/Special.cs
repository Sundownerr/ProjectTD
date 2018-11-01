
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower.Data.Stats
{
    public abstract class Special : ScriptableObject
    {
        public string SpecialName, SpecialDescription;

        public virtual void IncreaseStatsPerLevel() { }
        public virtual void InitSpecial(TowerSystem ownerTower) { }
    }
}
