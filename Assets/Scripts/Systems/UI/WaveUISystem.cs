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
           
            var creepList   = GM.I.WaveSystem.CurrentWaveCreepList;

            Race.text   = creepList[0].Race.ToString();
            Armor.text  = creepList[0].ArmorType.ToString();
            CreepTypes.text = CalculateTypes();
            Traits.text = "";

            #region  Helper functions

            string GetTraitsAndAbilities()
            {
               
                
                return "";
            }

            string CalculateTypes()
            {
                var creepTypes = new StringBuilder();
                var smallCount = 0;
                var normalCount = 0;
                var commanterCount = 0;
                var flyingCount = 0;
                var bossCount = 0;

                for (int i = 0; i < creepList.Count; i++)
                {
                    if (creepList[i] is Small)   {   smallCount++; continue;}
                    if (creepList[i] is Normal)  {   normalCount++; continue;}
                    if (creepList[i] is Commander){  commanterCount++; continue;}
                    if (creepList[i] is Flying)  {   flyingCount++; continue;}
                    if (creepList[i] is Boss)    {   bossCount++; continue;}            
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
