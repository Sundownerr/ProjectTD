using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class MultishotSystem : ITraitSystem
    {
        private Multishot trait;

        public MultishotSystem(Multishot trait) 
        {
            this.trait = trait;
        }

        public void IncreaseStatsPerLevel()
        {
            Debug.Log("increase stats per level");
        }
    }
}