using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;
using System;

namespace Game.Data
{	
	[CreateAssetMenu(fileName = "WaveDataBase", menuName = "Data/Wave Data Base")]

	[Serializable]
	public class WaveDataBase : ScriptableObject
	{
		[SerializeField]
		public List<Wave> WaveList;
	}
}
