using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Creep.Data;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "CreepDB", menuName = "Data/Creep DataBase")]
   	[Serializable]
	public class CreepDataBase : ScriptableObject, IData
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

