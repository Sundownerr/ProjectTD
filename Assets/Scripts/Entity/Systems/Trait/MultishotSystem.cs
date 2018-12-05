using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class MultishotSystem : TraitSystem
    {
        private new Multishot trait;

        public MultishotSystem(Multishot trait, EntitySystem owner) : base(trait, owner)
        {
            this.trait = trait;
        }

        public override void IncreaseStatsPerLevel()
        {
            Debug.Log("increase stats per level");
        }
    }
}