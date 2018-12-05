using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class TraitSystem 
	{
		public EntitySystem Owner { get => owner; set => owner = value; }
        public Trait Trait { get => trait; set => trait = value; }

        protected EntitySystem owner;
		protected Trait trait;

		public TraitSystem(Trait trait, EntitySystem owner)
		{
			this.trait = trait;
			this.owner = owner;
		}

        public virtual void IncreaseStatsPerLevel() {}	
    }

    public class BulletTraitSystem : TraitSystem
    {
        public BulletTraitSystem(Trait trait, EntitySystem owner) : base(trait, owner)
        {
        }
		
		public new virtual void IncreaseStatsPerLevel()
		{
		}

		public virtual void Apply(BulletSystem bullet) {}
    }
}