using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Creep;
using Game.Systems;
using Game.Tower;
using UnityEngine;

public class HealthSystem 
{
	public event EventHandler<CreepData> CreepDied = delegate{};

	private EntitySystem owner;
	private float maxHealth, healthRegen, regenTimer;

	public HealthSystem(EntitySystem owner)
	{
		this.owner = owner;
		if (owner is CreepSystem creep)
			maxHealth = creep.Stats.Health;
	}

	public void Update()
	{
		if (owner is CreepSystem creep)
		{
			var health = creep.Stats.Health;
			healthRegen = creep.Stats.HealthRegen;				
				
			if (health < maxHealth)
			{
				regenTimer = regenTimer > 1 ? 0 : regenTimer += Time.deltaTime;

				if (regenTimer == 1)
					health += healthRegen;
			}				
			else
				if (health > maxHealth)
					health = maxHealth;			
		}		
	}

	public void ChangeHealth(EntitySystem changer, float damage)
	{
		if (owner is CreepSystem creep)
		{			
			creep.LastDamageDealer = changer;
			creep.Stats.Health -= damage;
			
			if (creep.Stats.Health <= 0)
				GiveResources();			
		}


		#region  Helper functions
		
		void GiveResources()
        {
            if (creep.LastDamageDealer is TowerSystem tower)
            {
                tower.AddExp(creep.Stats.Exp);
				CreepDied?.Invoke(this, creep.Stats);             
            }
            CreepControlSystem.DestroyCreep(creep);
        }
		
		#endregion
	}
}
