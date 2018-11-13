using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public static class DamageSystem 
	{
		public static void DoDamage(EntitySystem target, float damage, EntitySystem dealer)
		{
			if(target is CreepSystem creep)							
				creep.HealthSystem.ChangeHealth(dealer, CalculateDamage());													
			
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