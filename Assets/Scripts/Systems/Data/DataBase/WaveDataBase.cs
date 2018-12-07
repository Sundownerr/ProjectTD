using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "WaveDataBase", menuName = "Data/Data Base/Wave Data Base")]

	[Serializable]
	public class WaveDataBase : ScriptableObject, IData
	{
		[SerializeField]
		public List<Wave> Waves;
	}
}
