using System;
using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
    public class WaveCreatingSystem
    {
        public List<CreepData> CreateWave(RaceType race, int WaveCount, Wave wave)
        {
            var raceList = GM.Instance.CreepDataBase.AllCreepList;       
            var fittingCreepList = new List<CreepData>();      

            for (int raceId = 0; raceId < raceList.Count; raceId++)
                if(raceId == (int)race)
                    for (int i = 0; i < raceList[raceId].CreepList.Count; i++)
                        if(raceList[raceId].CreepList[i].WaveLevel >= GM.Instance.WaveSystem.WaveNumber)
                            fittingCreepList.Add(raceList[raceId].CreepList[i]);                  
                    
             
            return GetFittingCreepList(fittingCreepList, wave);
        }

        private List<CreepData> GetFittingCreepList(List<CreepData> fittingCreepList, Wave wave)
        {
            var sortedCreepList = new List<CreepData>();
            var choosedCreepList = new CreepData[]
            {
                ChooseCreep<Small>(fittingCreepList), 
                ChooseCreep<Normal>(fittingCreepList), 
                ChooseCreep<Commander>(fittingCreepList), 
                ChooseCreep<Flying>(fittingCreepList), 
                ChooseCreep<Boss>(fittingCreepList)
            };

            for (int i = 0; i < wave.CreepTypeList.Count; i++)        
                sortedCreepList.Add(GetCreepOfType(wave.CreepTypeList[i]));                     
            return sortedCreepList;

            CreepData GetCreepOfType(CreepType type)
            {         
                for (int i = 0; i < choosedCreepList.Length; i++)
                    if(choosedCreepList[i].Type == type)
                        return choosedCreepList[i];        
                return null;
            }
        }

        private CreepData ChooseCreep<T> (List<CreepData> fittingCreepList) where T : CreepData
        {        
            var choosedCreepList = new List<CreepData>();           
    
            for (int i = 0; i < fittingCreepList.Count; i++)           
                if(fittingCreepList[i] is T fittingCreep)
                    if(choosedCreepList.Count < 2)
                        choosedCreepList.Add(fittingCreep);
                    else
                        for (int j = 0; j < choosedCreepList.Count; j++)
                            if(fittingCreep != choosedCreepList[j])
                            {
                                choosedCreepList.Add(fittingCreep);  
                                break;
                            }

            var random = StaticRandom.Instance.Next(0, choosedCreepList.Count);
            return choosedCreepList.Count > 0 ? choosedCreepList[random] : null;                                                                   
        }
    }
}
