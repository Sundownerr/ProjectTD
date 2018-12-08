using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Creep.System
{
	public class AbilityControlSystem 
	{
		private CreepSystem creep;
	
		public AbilityControlSystem(CreepSystem ownerCreep) => creep = ownerCreep;

        public void Set() {}

        public void UpdateSystem()
        {
            var abilities = creep.Stats.Abilities;
            var abilitySystems = creep.AbilitySystems;   
            
            if (abilitySystems.Count < abilities.Count)                          
                abilitySystems.Add(new AbilitySystem(abilities[abilitySystems.Count], creep));           

			for (int i = 0; i < abilitySystems.Count; i++)			
				abilitySystems[i].Init();		
        }
	}
}
