using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "CreepTraitDB", menuName = "Data/Data Base/Creep Trait DataBase")]

	public class CreepTraitDataBase : ScriptableObject
	{
		public List<Trait> Traits;
	}
}
