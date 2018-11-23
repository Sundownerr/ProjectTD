using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Data;
using Game.Tower.Data.Stats;
using Newtonsoft.Json;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Systems
{
	public static class DataLoadingSystem 
	{
		public static PlayerData LoadPlayerData()
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

                var data = JsonConvert.SerializeObject(playerData);

                File.Create("playerData.json").Dispose();            
                File.WriteAllText("playerData.json", data);
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
	}
}
