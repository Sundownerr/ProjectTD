using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Creep.System
{
	public class AbilityControlSystem 
	{
		private CreepSystem creep;
		private List<AbilitySystem> abilitySystems;

		public AbilityControlSystem(CreepSystem ownerCreep)
        {
            creep = ownerCreep;
        }

        public void Set()
        {
            abilitySystems = creep.AbilitySystems;         
        }

        public void UpdateSystem()
        {
			for (int i = 0; i < abilitySystems.Count; i++)			
				abilitySystems[i].Init();		
        }
	}
}
