using System;
using System.Collections.Generic;
using Game.Creep;
using Game.Creep.Data;
using Game.Data;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Systems
{
    public static class WaveCreatingSystem
    {
        public static List<CreepData> CreateWave(Wave wave, int waveNumber)
        {
            var races           = GM.I.CreepDataBase.CreepRaces;   
            var waveRace        = RaceType.Humanoid;              
            var fittingCreeps   = new List<CreepData>();               
            var armorTypes      = Enum.GetValues(typeof(Armor.ArmorType));
            var randomArmorId   = StaticRandom.Instance.Next(0, armorTypes.Length);  

            for (int i = 0; i < races.Count; i++)
                if (i == (int)waveRace)
                    for (int j = 0; j < races[i].Creeps.Count; j++)
                        if (races[i].Creeps[j].WaveLevel >= GM.I.WaveSystem.WaveNumber)
                            fittingCreeps.Add(races[i].Creeps[j]);                  
                               
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

                var traits = GetRandomTraits();
                var abilities = GetRandomAbilities();

                for (int i = 0; i < wave.CreepTypes.Count; i++)       
                { 
                    var creep = GetCreepOfType(wave.CreepTypes[i]);
                    creep = U.Instantiate(creep);

                    CalculateStats(creep);

                    sortedCreeps.Add(creep);  
                } 

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

                void CalculateStats(CreepData stats)
                {                                   
                    stats.ArmorType         = (Armor.ArmorType)armorTypes.GetValue(randomArmorId);
                    stats.ArmorValue        = waveNumber;
                    stats.DefaultMoveSpeed  = 120 + waveNumber * 5;
                    stats.Gold              = 1 + waveNumber;        // waveCount / 7;
                    stats.Health            = waveNumber * 10;
                    stats.MoveSpeed         = stats.DefaultMoveSpeed;   

                    stats.Traits = new List<Trait>();
                    stats.Traits.AddRange(traits);

                    if(stats is Boss || stats is Commander)    
                    {
                        stats.Abilities = new List<Ability>();
                        stats.Abilities.AddRange(abilities); 
                    }

                    stats.IsInstanced = true;     
                }         

                List<Ability> GetRandomAbilities() 
                {                        
                    var randomAbilityCount = StaticRandom.Instance.Next(0, 2);
                    var randomAbilities = new List<Ability>();

                    for (int i = 0; i < randomAbilityCount; i++)
                    {
                        var isHaveSameAbility = false;
                        var randomAbilityId = StaticRandom.Instance.Next(0, GM.I.CreepAbilityDataBase.Abilities.Count);
                        var randomAbility = GM.I.CreepAbilityDataBase.Abilities[randomAbilityId];

                        for (int j = 0; j < randomAbilities.Count; j++)
                            if (randomAbilities[j].CompareId(randomAbility.Id))
                            {
                                isHaveSameAbility = true;
                                break;
                            }
                        
                        if (!isHaveSameAbility)
                            randomAbilities.Add(randomAbility);
                    }                        
                    return randomAbilities;
                } 

                List<Trait> GetRandomTraits()
                {
                    var randomTraitCount = StaticRandom.Instance.Next(0, 2);
                    var randomTraits = new List<Trait>();
            
                    for (int i = 0; i < randomTraitCount; i++)
                    {
                        var isHaveSameTrait = false;
                        var randomTraitId = StaticRandom.Instance.Next(0, GM.I.CreepTraitDataBase.Traits.Count);
                        var randomTrait = GM.I.CreepTraitDataBase.Traits[randomTraitId];

                        for (int j = 0; j < randomTraits.Count; j++)
                            if (randomTraits[j].CompareId(randomTrait.Id))
                            {
                                isHaveSameTrait = true;
                                break;
                            }
                        
                        if (!isHaveSameTrait)
                            randomTraits.Add(randomTrait);
                    }         
                    return randomTraits;
                }         
                #endregion                 
            }      
            #endregion
        }
    }
}
