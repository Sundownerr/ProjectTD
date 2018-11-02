
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.System;
using Game.Creep.Data;

namespace Game.Data
{
  	[CreateAssetMenu(fileName = "CreepDataBase", menuName = "Data/Creep Data Base")]
   	[Serializable]
	public class CreepDataBase : ScriptableObject 
	{
        [SerializeField]
        public List<RaceType> AllCreepList;

        private void Awake()
        {

            var creepDB = Resources.Load("CreepDataBase");
            
            if(creepDB is CreepDataBase data)
            {
                AllCreepList = data.GetAllTowerList();
            }
            else 
            {
                AllCreepList = new List<RaceType>();                
                                    
                   
				var raceList = Enum.GetValues(typeof(RaceType));  
                         
                foreach (RaceType race in raceList)
				{
					AllCreepList.Add(race);
				}
            }                            
        }

        public List<RaceType> GetAllTowerList() => AllCreepList;              
    }
}

