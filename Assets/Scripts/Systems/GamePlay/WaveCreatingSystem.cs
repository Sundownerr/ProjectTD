using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using Game.Data;
using Game.System;
using UnityEngine;

namespace Game.System
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
                            allCreepList[raceId].CreepList[i].WaveLevel >= GM.Instance.WaveSystem.WaveCount;

                        if(isCreepOk)
                            fittingCreepList.Add(allCreepList[raceId].CreepList[i]);                  
                    }      

            return GetFittingCreepList(fittingCreepList, wave);
        }

        private List<CreepData> GetFittingCreepList(List<CreepData> fittingCreepList, Wave wave)
        {
            var fittingCreepIndexList = new List<int>();
            var tempCreepList = new List<CreepData>();

            for (int i = 0; i < wave.CreepTypeList.Count; i++)
            {
                fittingCreepIndexList.Clear();

                for (int j = 0; j < fittingCreepList.Count; j++)
                    if(wave.CreepTypeList[i] == fittingCreepList[j].Type)
                        fittingCreepIndexList.Add(j);       

                if(fittingCreepIndexList.Count > 1)
                    tempCreepList.Add(fittingCreepList[Random.Range(0, fittingCreepIndexList.Count)]);
                else
                    tempCreepList.Add(fittingCreepList[fittingCreepIndexList[0]]);
            }

            return tempCreepList;
        }
    }
}
