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
        public static List<CreepData> CreateWave(RaceType waveRace, Wave wave)
        {
            var raceList = GM.I.CreepDataBase.AllCreepList;       
            var fittingCreepList = new List<CreepData>();      

            for (int raceId = 0; raceId < raceList.Count; raceId++)
                if (raceId == (int)waveRace)
                    for (int i = 0; i < raceList[raceId].CreepList.Count; i++)
                        if (raceList[raceId].CreepList[i].WaveLevel >= GM.I.WaveSystem.WaveNumber)
                            fittingCreepList.Add(raceList[raceId].CreepList[i]);                  
                               
            return GetFittingCreepList();

            #region Helper functions

            List<CreepData> GetFittingCreepList()
            {
                var sortedCreepList = new List<CreepData>();
                var choosedCreepList = new CreepData[]
                {
                    ChooseCreep<Small>(), 
                    ChooseCreep<Normal>(), 
                    ChooseCreep<Commander>(), 
                    ChooseCreep<Flying>(), 
                    ChooseCreep<Boss>()
                };

                for (int i = 0; i < wave.CreepTypeList.Count; i++)        
                    sortedCreepList.Add(GetCreepOfType(wave.CreepTypeList[i]));   

                return sortedCreepList;

                #region Helper functions

                CreepData GetCreepOfType(CreepData creep) 
                {         
                    for (int i = 0; i < choosedCreepList.Length; i++)
                        if (choosedCreepList[i].GetType() == creep.GetType())
                            return choosedCreepList[i];        
                    return null;
                }

                CreepData ChooseCreep<T>() where T : CreepData
                {        
                    var tempChoosedCreepList = new List<CreepData>();           
            
                    for (int i = 0; i < fittingCreepList.Count; i++)           
                        if (fittingCreepList[i] is T fittingCreep)
                            if (tempChoosedCreepList.Count < 2)
                                tempChoosedCreepList.Add(fittingCreep);
                            else
                                for (int j = 0; j < tempChoosedCreepList.Count; j++)
                                    if (fittingCreep != tempChoosedCreepList[j])
                                    {
                                        tempChoosedCreepList.Add(fittingCreep);  
                                        break;
                                    }

                    var random = StaticRandom.Instance.Next(0, tempChoosedCreepList.Count);
                    return tempChoosedCreepList.Count > 0 ? tempChoosedCreepList[random] : null;                                                                   
                }

                #endregion
                #endregion
            
            }    
        }
    }
}
