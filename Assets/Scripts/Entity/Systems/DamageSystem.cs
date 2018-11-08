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
					creep.Stats.Health -= CalculateDamage(creep, damage, dealer);

					if (creep.Stats.Health <= 0)
					{
						creep.IsKilled = true;
						CreepControlSystem.GiveResources(creep);
					}
				}				
		}

		private static float CalculateDamage(EntitySystem target, float rawDamage, EntitySystem damageDealer)
        {
            var damage = 0f;

            if(damageDealer is TowerSystem tower)    
				if(target is CreepSystem creep)   
               		damage = ExtendedMonoBehaviour.GetPercentOfValue(tower.Stats.DamageToRace[(int)creep.Stats.Race], rawDamage);

            // add armor modificator

            return damage;
        }
	}
}