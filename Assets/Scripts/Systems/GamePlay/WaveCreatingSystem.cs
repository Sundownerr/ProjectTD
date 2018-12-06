using System;
using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
    public static class WaveCreatingSystem
    {
        public static List<CreepData> CreateWave(Wave wave)
        {
            var races = GM.I.CreepDataBase.CreepRaces;   
            var waveRace = RaceType.Humanoid;    
            
            var fittingCreeps = new List<CreepData>();      

            for (int raceId = 0; raceId < races.Count; raceId++)
                if (raceId == (int)waveRace)
                    for (int i = 0; i < races[raceId].Creeps.Count; i++)
                        if (races[raceId].Creeps[i].WaveLevel >= GM.I.WaveSystem.WaveNumber)
                            fittingCreeps.Add(races[raceId].Creeps[i]);                  
                               
            return GetFittingCreeps();

            #region Helper functions

            List<CreepData> GetFittingCreeps()
            {
                var sortedCreeps = new List<CreepData>();
                var choosedCreeps = new CreepData[]
                {
                    ChooseCreep<Small>(), 
                    ChooseCreep<Normal>(), 
                    ChooseCreep<Commander>(), 
                    ChooseCreep<Flying>(), 
                    ChooseCreep<Boss>()
                };

                for (int i = 0; i < wave.CreepTypes.Count; i++)        
                    sortedCreeps.Add(GetCreepOfType(wave.CreepTypes[i]));   

                return sortedCreeps;

                #region Helper functions

                CreepData GetCreepOfType(CreepData creep) 
                {         
                    for (int i = 0; i < choosedCreeps.Length; i++)
                        if (choosedCreeps[i].GetType() == creep.GetType())
                            return choosedCreeps[i];        
                    return null;
                }

                CreepData ChooseCreep<T>() where T : CreepData
                {        
                    var tempChoosedCreeps = new List<CreepData>();           
            
                    for (int i = 0; i < fittingCreeps.Count; i++)           
                        if (fittingCreeps[i] is T fittingCreep)
                            if (tempChoosedCreeps.Count < 2)
                                tempChoosedCreeps.Add(fittingCreep);
                            else
                                for (int j = 0; j < tempChoosedCreeps.Count; j++)
                                    if (fittingCreep != tempChoosedCreeps[j])
                                    {
                                        tempChoosedCreeps.Add(fittingCreep);  
                                        break;
                                    }

                    var random = StaticRandom.Instance.Next(0, tempChoosedCreeps.Count);
                    return tempChoosedCreeps.Count > 0 ? tempChoosedCreeps[random] : null;                                                                   
                }

                #endregion                 
            }    

            #endregion
        }
    }
}
