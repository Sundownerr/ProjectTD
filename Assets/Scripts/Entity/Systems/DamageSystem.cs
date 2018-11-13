using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class DamageSystem 
	{
		public static void DoDamage(EntitySystem target, float damage, EntitySystem dealer)
		{
			if(target is CreepSystem creep)			
				if (!creep.IsKilled)
            	{
					creep.LastDamageDealer = dealer;
					creep.Stats.Health -= CalculateDamage();
					creep.IsKilled = creep.Stats.Health <= 0;

					if(creep.IsKilled)
						CreepControlSystem.GiveResources(creep);					
				}
			
			float CalculateDamage()
			{				
				if(dealer is TowerSystem tower)   					
					damage = QoL.GetPercentOfValue(tower.Stats.DamageToRace[(int)creep.Stats.Race], damage);

				// add armor modificator

				return damage;
			}				
		}		
	}
}