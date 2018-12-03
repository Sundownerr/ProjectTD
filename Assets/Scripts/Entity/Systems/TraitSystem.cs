using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
	public abstract class TraitSystem 
	{
		public EntitySystem Owner { get => owner; set => owner = value; }
        public Trait Trait { get => trait; set => trait = value; }

        protected EntitySystem owner;
		private Trait trait;

        public TraitSystem(EntitySystem owner, Trait trait)
		{
			this.owner = owner;
			this.trait = trait;
		} 
		
		public virtual void Init() {}
		public virtual void IncreaseStatsPerLevel() { }     
	}
}