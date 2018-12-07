using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Game.Creep.Data;
using System.Text;

namespace Game.Systems
{
    public class WaveUISystem : ExtendedMonoBehaviour
    {
        public TextMeshProUGUI CreepTypes, Race, Armor, Traits;

        protected override void Awake()
        {
            base.Awake();
            GM.I.WaveUISystem = this;
        }

        public void SetSystem()
        {          
            GM.I.WaveSystem.WaveChanged += UpdateWaveUI;          
            UpdateWaveUI(this, new EventArgs());  
        }

        public void UpdateWaveUI(object sender, EventArgs e)
        {          
            var creeps  = GM.I.WaveSystem.CurrentWaveCreeps;

            Race.text   = creeps[0].Race.ToString();
            Armor.text  = creeps[0].ArmorType.ToString();
            CreepTypes.text = CalculateTypes();
            Traits.text = GetTraitsAndAbilities();

            #region  Helper functions

            string GetTraitsAndAbilities()
            {
                var traitsAndAbilities = new StringBuilder();

                for (int i = 0; i < creeps.Count; i++)               
                    if(creeps[i] is Commander || creeps[i] is Boss)
                    {
                        for (int j = 0; j < creeps[i].Traits.Count; j++)                        
                            traitsAndAbilities.Append($"{creeps[i].Traits[j].Name} ");

                        for (int j = 0; j < creeps[i].Abilities.Count; j++)                        
                            traitsAndAbilities.Append($"{creeps[i].Abilities[j].Name} ");              
                    }             
                return traitsAndAbilities.ToString();
            }

            string CalculateTypes()
            {
                var creepTypes = new StringBuilder();
                var smallCount = 0;
                var normalCount = 0;
                var commanterCount = 0;
                var flyingCount = 0;
                var bossCount = 0;

                for (int i = 0; i < creeps.Count; i++)
                {
                    if (creeps[i] is Small)   {   smallCount++; continue;}
                    if (creeps[i] is Normal)  {   normalCount++; continue;}
                    if (creeps[i] is Commander){  commanterCount++; continue;}
                    if (creeps[i] is Flying)  {   flyingCount++; continue;}
                    if (creeps[i] is Boss)    {   bossCount++; continue;}            
                }

                creepTypes
                    .Append(smallCount > 0    ? $"{smallCount} Small " : string.Empty)
                    .Append(normalCount > 0   ? $"{normalCount} Normal " : string.Empty)
                    .Append(commanterCount > 0 ? $"{commanterCount} Commander " : string.Empty)
                    .Append(flyingCount > 0   ? $"{flyingCount} Flying " : string.Empty)
                    .Append(bossCount > 0     ? $"{bossCount} Boss " : string.Empty);
                return creepTypes.ToString();
            }        

            #endregion
        }        
    }
}
