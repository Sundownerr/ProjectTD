
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
            if(AllCreepList == null)    
            {      
                AllCreepList = new List<Race>();             

				var raceList = Enum.GetValues(typeof(RaceType));  
                
                for (int i = 0; i < raceList.Length; i++)
                    AllCreepList.Add(new Race());               
            }                           
        }              
    }
}

