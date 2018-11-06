using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using Game.Data;

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
                        if(allCreepList[raceId].CreepList[i].WaveLevel >= GM.Instance.WaveSystem.WaveNumber)
                            fittingCreepList.Add(allCreepList[raceId].CreepList[i]);                  
                    
            return GetFittingCreepList(fittingCreepList, wave);
        }

        private List<CreepData> GetFittingCreepList(List<CreepData> fittingCreepList, Wave wave)
        {
            var tempCreepList = new List<CreepData>();
            var creepList = new CreepData[]
            {
                GetFittingCreepOfType(CreepType.Small, fittingCreepList), 
                GetFittingCreepOfType(CreepType.Normal, fittingCreepList), 
                GetFittingCreepOfType(CreepType.Commander, fittingCreepList), 
                GetFittingCreepOfType(CreepType.Boss, fittingCreepList)
            };
 
            for (int i = 0; i < wave.CreepTypeList.Count; i++)        
                tempCreepList.Add(GetFittingCreep(wave.CreepTypeList[i], creepList));                     

            return tempCreepList;
        }

        private CreepData GetFittingCreep(CreepType neededType, CreepData[] creepList)
        {
            for (int i = 0; i < creepList.Length; i++)          
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
