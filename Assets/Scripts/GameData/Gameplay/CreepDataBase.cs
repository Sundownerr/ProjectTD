
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Systems;
using Game.Creep.Data;

namespace Game.Data
{
  	[CreateAssetMenu(fileName = "CreepDataBase", menuName = "Data/Creep Data Base")]
   	[Serializable]
	public class CreepDataBase : ScriptableObject 
	{
        [SerializeField]
        public List<Race> AllCreepList;

        private void Awake()
        {
            var creepDB = Resources.Load("CreepDataBase");
            
            if(creepDB is CreepDataBase data)            
                AllCreepList = data.GetAllTowerList();          
            else 
            {               
                AllCreepList = new List<Race>();             

				var raceList = Enum.GetValues(typeof(RaceType));  
                
                for (int i = 0; i < raceList.Length; i++)
                    AllCreepList.Add(new Race());
                
            }                           
        }

        public List<Race> GetAllTowerList() => AllCreepList;              
    }
}

