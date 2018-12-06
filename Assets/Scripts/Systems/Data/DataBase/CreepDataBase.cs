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
        public List<Race> CreepRaces;

        private void Awake()
        {
            if (CreepRaces == null)    
            {      
                CreepRaces = new List<Race>();             

				var races = Enum.GetValues(typeof(RaceType));  
                
                for (int i = 0; i < races.Length; i++)
                    CreepRaces.Add(new Race());               
            }                           
        }  
    }
}

