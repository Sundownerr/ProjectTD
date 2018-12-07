using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Data;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class ArmoredSystem : ICreepTraitSystem
    {
        public EntitySystem Owner { get => owner; set => owner = value; }

        private EntitySystem owner;
        private Armored trait;

        public ArmoredSystem(Armored trait, EntitySystem owner) 
        {
            this.trait = trait;
            Owner = owner;
        }

        public void IncreaseStatsPerLevel()
        {
            //Debug.Log("increase stats per level");
        }

		public void Apply(CreepSystem creep)
		{
			creep.Stats.ArmorValue += trait.AdditionalArmor;
		}
    }
}
