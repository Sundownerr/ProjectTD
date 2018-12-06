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
        public EntitySystem Owner { get => owner; set => owner = value; }

        private EntitySystem owner;
        private Multishot trait;

        public MultishotSystem(Multishot trait, EntitySystem owner) 
        {
            this.trait = trait;
            Owner = owner;
        }

        public void IncreaseStatsPerLevel()
        {
            //Debug.Log("increase stats per level");
        }
    }
}