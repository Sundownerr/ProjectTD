using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Data;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public interface ITraitSystem
	{
		EntitySystem Owner {get; set;}
		void IncreaseStatsPerLevel();
	}

	public interface IBulletTraitSystem : ITraitSystem
	{
		void Apply(BulletSystem bullet);
	}

	public interface ICreepTraitSystem : ITraitSystem
	{
		void Apply(CreepSystem creep);
	}
}