using System.Collections.Generic;
using UnityEngine;
using System;
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

        private void OnValidate()
        {
            for (int i = 0; i < AllCreepList.Count; i++)       
                for (int j = 0; j < AllCreepList[i].CreepList.Count; j++)              
                    if(AllCreepList[i].CreepList[j] == null)
                        AllCreepList[i].CreepList.RemoveAt(j);                          
        }                  
    }
}

