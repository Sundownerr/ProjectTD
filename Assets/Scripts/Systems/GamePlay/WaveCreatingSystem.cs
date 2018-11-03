using System;
using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using Game.Data;
using Game.Systems;
using UnityEngine;

namespace Game.Systems
{
   
    public class WaveCreatingSystem
    {
        public List<CreepData> CreateWave(RaceType race, int WaveCount, Wave wave)
        {
            var allCreepList = GM.Instance.CreepDataBase.AllCreepList;       
            var fittingCreepList = new List<CreepData>();      

            for (int raceId = 0; raceId < allCreepList.Count; raceId++)
                if(raceId == (int)race)
                    for (int i = 0; i < allCreepList[raceId].CreepList.Count; i++)
                    {
                        var isCreepOk =
                            allCreepList[raceId].CreepList[i].WaveLevel >= GM.Instance.WaveSystem.WaveNumber;

                        if(isCreepOk)
                            fittingCreepList.Add(allCreepList[raceId].CreepList[i]);                  
                    }   

            return GetFittingCreepList(fittingCreepList, wave);
        }

        private List<CreepData> GetFittingCreepList(List<CreepData> fittingCreepList, Wave wave)
        {
            var fittingCreepIndexList = new List<int>();
            var tempCreepList = new List<CreepData>();

            var smallCreep      = GetFittingCreepOfType(CreepType.Small, fittingCreepList);
            var normalCreep     = GetFittingCreepOfType(CreepType.Normal, fittingCreepList);
            var commanderCreep  = GetFittingCreepOfType(CreepType.Commander, fittingCreepList);
            var bossCreep       = GetFittingCreepOfType(CreepType.Boss, fittingCreepList);

            var creepList = new List<CreepData>() {smallCreep, normalCreep, commanderCreep, bossCreep};
 
            for (int i = 0; i < wave.CreepTypeList.Count; i++)        
                tempCreepList.Add(GetFittingCreep(wave.CreepTypeList[i], creepList));                     

            return tempCreepList;
        }

        private CreepData GetFittingCreep(CreepType neededType, List<CreepData> creepList)
        {
            for (int i = 0; i < creepList.Count; i++)          
                if(creepList[i].Type == neededType)
                    return creepList[i];
            
            return null;
        }

        private CreepData GetFittingCreepOfType(CreepType type, List<CreepData> fittingCreepList)
        {
            var tempCreepOfTypeList = new List<CreepData>();           
    
            for (int i = 0; i < fittingCreepList.Count; i++)
                if(fittingCreepList[i].Type == type)
                    if(tempCreepOfTypeList.Count < 2)
                        tempCreepOfTypeList.Add(fittingCreepList[i]);
                    else
                        for (int j = 0; j < tempCreepOfTypeList.Count; j++)
                            if(fittingCreepList[i] != tempCreepOfTypeList[j])
                            {
                                tempCreepOfTypeList.Add(fittingCreepList[i]);  
                                break;
                            }

            return tempCreepOfTypeList.Count > 0 ? tempCreepOfTypeList[StaticRandom.Instance.Next(0, tempCreepOfTypeList.Count)] : null;                                                                   
        }
    }
}
