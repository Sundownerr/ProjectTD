using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Tower
{
    public abstract class Special : ScriptableObject
    {
        public string SpecialName, SpecialDescription;

        public virtual void IncreaseStatsPerLevel() { }
        public virtual void InitSpecial(TowerBaseSystem ownerTower) { }
    }
}
