using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Data;
using Game.Tower.Data.Stats;
using Newtonsoft.Json;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Systems
{
	public static class DataLoadingSystem 
	{		
		public static void Save<T>(T data) where T : IData
		{		
			if (data is PlayerData)			
			{
				var newData = JsonConvert.SerializeObject(data);
				File.WriteAllText("playerData.json", newData);
				return;
			}
#if UNITY_EDITOR			
			if (data is TowerDataBase)		
			{	
				EditorUtility.SetDirty(data as TowerDataBase); 
				return;
			}

			if (data is CreepDataBase)		
			{	
				EditorUtility.SetDirty(data as CreepDataBase); 						
				return;
			}
#endif
		}

		public static IData Load<T>() where T : IData
		{
			return 
				typeof(T) == typeof(PlayerData) ? LoadPlayerData() :
#if UNITY_EDITOR
				typeof(T) == typeof(TowerDataBase) 	? LoadTowerDB() :
				typeof(T) == typeof(CreepDataBase) 	? LoadCreepDB() :
#endif
				null as IData;

			PlayerData LoadPlayerData()
			{
				var playerData = new PlayerData();

				if (!File.Exists("playerData.json"))
				{
					playerData.ElementLevelList = new List<int>();
					var elementAmount = Enum.GetValues(typeof(ElementType)).Length;

					for (int i = 0; i < elementAmount; i++)
						playerData.ElementLevelList.Add(0);      

					playerData.MaxTowerLimit = 500;
					playerData.StartTowerRerollCount = 3;
					playerData.MagicCrystals = 100;   
					playerData.Gold = 100;

					var newData = JsonConvert.SerializeObject(playerData);
			
					File.WriteAllText("playerData.json", newData);
				} 
				else
				{
					var dataFromFile = File.ReadAllText("playerData.json");				
					var loadedData = JsonConvert.DeserializeObject<PlayerData>(dataFromFile);
					
					playerData.ElementLevelList 	= new List<int>();
					playerData.ElementLevelList    	= loadedData.ElementLevelList;
					playerData.MagicCrystals       	= loadedData.MagicCrystals;
					playerData.Gold                	= loadedData.Gold;
					playerData.CurrentTowerLimit   	= loadedData.CurrentTowerLimit;
					playerData.MaxTowerLimit       	= loadedData.MaxTowerLimit;				
				}        

				return playerData;
			}		
#if UNITY_EDITOR
			TowerDataBase LoadTowerDB() =>					
				AssetDatabase.LoadAssetAtPath("Assets/DataBase/TowerDB.asset", typeof(TowerDataBase)) as TowerDataBase;

			CreepDataBase LoadCreepDB() =>					
				AssetDatabase.LoadAssetAtPath("Assets/DataBase/CreepDB.asset", typeof(CreepDataBase)) as CreepDataBase;
#endif
		}
	}
}
