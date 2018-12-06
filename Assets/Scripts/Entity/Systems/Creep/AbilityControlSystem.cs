using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Creep.System
{
	public class AbilityControlSystem 
	{
		private CreepSystem creep;
	

		public AbilityControlSystem(CreepSystem ownerCreep)
        {
            creep = ownerCreep;
        }

        public void Set()
        {
                  
        }

        public void UpdateSystem()
        {
            var abilitySystems = creep.AbilitySystems;   
            
			for (int i = 0; i < abilitySystems.Count; i++)			
				abilitySystems[i].Init();		
        }
	}
}
