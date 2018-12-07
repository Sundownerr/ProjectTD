using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "CreepAbilityDB", menuName = "Data/Data Base/Creep Ability DataBase")]

	public class CreepAbilityDataBase : ScriptableObject
	{
		public List<Ability> Abilities;
	}
}
