using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Creep.System
{
    public class TraitControlSystem
    {
		private CreepSystem creep;

        public TraitControlSystem(CreepSystem ownerCreep)
        {
			creep = ownerCreep;
        }

		public void UpdateSystem()
		{
			var traits = creep.Stats.Traits;
			var traitSystems = creep.TraitSystems;

			if(traitSystems.Count < traits.Count)
			{
				var newTraitSystem = traits[traitSystems.Count].GetTraitSystem(creep) as ICreepTraitSystem;
				traitSystems.Add(newTraitSystem);    
				newTraitSystem.Apply(creep);			
			}     
		}
    }
}