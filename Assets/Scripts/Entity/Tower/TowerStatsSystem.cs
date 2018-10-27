using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data.Entity.Tower;
using Game.System;

namespace Game.Tower
{
    public class TowerStatsSystem 
    {
        public TowerData Stats, BaseStats;
 
        private TowerBaseSystem ownerTower;      
        private List<TowerData> gradeList;
        private int gradeCount;
        private StateMachine state;

        public TowerStatsSystem(TowerBaseSystem ownerTower)
        {
            this.ownerTower = ownerTower;

            Stats = Object.Instantiate(ownerTower.Stats);      
            BaseStats = Object.Instantiate(Stats);
            gradeList = BaseStats.GradeList;       

            for (int i = 0; i < Stats.AbilityList.Count; i++)
            {
                Stats.AbilityList[i] = Object.Instantiate(Stats.AbilityList[i]);

                for (int j = 0; j < Stats.AbilityList[i].EffectList.Count; j++)
                {
                    Stats.AbilityList[i].EffectList[j] = Object.Instantiate(Stats.AbilityList[i].EffectList[j]);
                }

                Stats.AbilityList[i].SetOwnerTower(ownerTower);
            }
        }      

        public void Upgrade(TowerData currentStats, TowerData newBaseStats)
        {
            Stats = Object.Instantiate(newBaseStats);
            Stats.Level = currentStats.Level;
            Stats.Exp = currentStats.Exp;

            for (int i = 1; i < Stats.Level; i++)
            {
                IncreaseStatsPerLevel();
            }

            BaseStats = Object.Instantiate(newBaseStats);
        }

        private void IncreaseStatsPerLevel()
        {
            Stats.Damage += Mathf.FloorToInt(ExtendedMonoBehaviour.GetPercentOfValue(4f, BaseStats.Damage));
            Stats.AttackSpeed -= ExtendedMonoBehaviour.GetPercentOfValue(1.2f, BaseStats.AttackSpeed);
            Stats.CritChance += ExtendedMonoBehaviour.GetPercentOfValue(0.2f, BaseStats.CritChance);
            Stats.SpellCritChance += ExtendedMonoBehaviour.GetPercentOfValue(0.2f, BaseStats.SpellCritChance);          
        }

        public void AddExp(int amount)
        {
            Stats.Exp += amount;

            for (int i = Stats.Level; i < 25; i++)
            {
                if (Stats.Exp >= GM.ExpToLevelUp[Stats.Level - 1] && Stats.Level < 25)
                {
                    IncreaseStatsPerLevel();
                    Stats.Level++;

                    var effect = Object.Instantiate(GM.Instance.LevelUpEffect, ownerTower.transform.position, Quaternion.identity);
                    Object.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                }
            }
            UpdateUI();
        }


        public void UpdateUI()
        {
            if (GM.Instance.PlayerInputSystem.ChoosedTower == ownerTower.gameObject)
            {
                GM.Instance.TowerUISystem.UpdateValues();
            }
        }
    }
}