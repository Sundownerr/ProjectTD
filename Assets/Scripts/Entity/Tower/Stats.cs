using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using System;
using Game.Tower.Data;

namespace Game.Tower.System
{
    [Serializable]
    public class Stats 
    {
        public TowerData CurrentStats, BaseStats;
 
        private TowerSystem tower;      

        public Stats(TowerSystem ownerTower) => tower = ownerTower;    
   
        public void Set()
        {
            CurrentStats = UnityEngine.Object.Instantiate(CurrentStats);

            for (int i = 0; i < CurrentStats.AbilityList.Count; i++)
            {
                CurrentStats.AbilityList[i] = UnityEngine.Object.Instantiate(CurrentStats.AbilityList[i]);

                for (int j = 0; j < CurrentStats.AbilityList[i].EffectList.Count; j++)
                    CurrentStats.AbilityList[i].EffectList[j] = UnityEngine.Object.Instantiate(CurrentStats.AbilityList[i].EffectList[j]);

                CurrentStats.AbilityList[i].SetOwnerTower(tower);
            }

            BaseStats = UnityEngine.Object.Instantiate(CurrentStats);
        }

        public void Upgrade(TowerData currentStats, TowerData newBaseStats)
        {
            CurrentStats = UnityEngine.Object.Instantiate(newBaseStats);
                       
            CurrentStats.Level = currentStats.Level;
            CurrentStats.Exp = currentStats.Exp;

            UpgradeSpecial(newBaseStats);

            BaseStats = UnityEngine.Object.Instantiate(newBaseStats);

            for (int i = 1; i < CurrentStats.Level; i++)
                IncreaseStatsPerLevel();
        }

        private void UpgradeSpecial(TowerData newBaseStats)
        {          
            for (int i = 0; i < newBaseStats.SpecialList.Length; i++)
                CurrentStats.SpecialList[i] = UnityEngine.Object.Instantiate(CurrentStats.SpecialList[i]);
        }

        private void IncreaseStatsPerLevel()
        {
            CurrentStats.Damage.Value += Mathf.FloorToInt(ExtendedMonoBehaviour.GetPercentOfValue(4f, BaseStats.Damage.Value));
            CurrentStats.AttackSpeed -= ExtendedMonoBehaviour.GetPercentOfValue(1.2f, BaseStats.AttackSpeed);
            CurrentStats.CritChance += ExtendedMonoBehaviour.GetPercentOfValue(0.2f, BaseStats.CritChance);
            CurrentStats.SpellCritChance += ExtendedMonoBehaviour.GetPercentOfValue(0.2f, BaseStats.SpellCritChance);
            
            tower.GetSpecial().IncreaseStatsPerLevel();
        }

        public void AddExp(int amount)
        {
            CurrentStats.Exp += amount;

            for (int i = CurrentStats.Level; i < 25; i++)
                if (CurrentStats.Exp >= GM.ExpToLevelUp[CurrentStats.Level - 1] && CurrentStats.Level < 25)
                {
                    IncreaseStatsPerLevel();                   

                    CurrentStats.Level++;

                    var effect = UnityEngine.Object.Instantiate(GM.Instance.LevelUpEffect, tower.transform.position, Quaternion.identity);
                    UnityEngine.Object.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                }           
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (GM.Instance.PlayerInputSystem.ChoosedTower == tower.gameObject)
                GM.Instance.TowerUISystem.UpdateValues();           
        }
    }
}