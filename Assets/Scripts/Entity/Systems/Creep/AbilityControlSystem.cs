using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Creep.System
{
	public class AbilityControlSystem 
	{
		private CreepSystem creep;
		private List<AbilitySystem> abilitySystemList;

		public AbilityControlSystem(CreepSystem ownerCreep)
        {
            creep = ownerCreep;
        }

        public void Set()
        {
            abilitySystemList = creep.AbilitySystemList;
           
        }

        public void UpdateSystem()
        {
			for (int i = 0; i < abilitySystemList.Count; i++)
			{
				abilitySystemList[i].Init();
			}
        }
	}
}
