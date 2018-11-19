using System;
using Game.Systems;
using Game.Tower.Data;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Tower.System
{
    [Serializable]
    public class Stats
    {
        public TowerData CurrentStats { get => currentStats; set => currentStats = value; }
        public TowerData BaseStats { get => baseStats; set => baseStats = value; }
        public event EventHandler StatsChanged = delegate {};

        private TowerSystem tower;
        private TowerData currentStats, baseStats;

        public Stats(TowerSystem ownerTower) => tower = ownerTower;

        public void Set() => Set(currentStats);

        public void Set(TowerData stats)
        {
            currentStats = U.Instantiate(stats);            
            currentStats.SetData();

            for (int i = 0; i < stats.AbilityList.Count; i++)
            {
                currentStats.AbilityList[i] = U.Instantiate(stats.AbilityList[i]);

                for (int j = 0; j < stats.AbilityList[i].EffectList.Count; j++)
                    currentStats.AbilityList[i].EffectList[j] = U.Instantiate(stats.AbilityList[i].EffectList[j]);

                currentStats.AbilityList[i].SetOwner(tower);
            }

            baseStats = U.Instantiate(stats);
            baseStats.IsInstanced = true;
            OnStatsChanged();
        }

        public void Upgrade(TowerData previousStats, TowerData newStats)
        {            
            Set(newStats);
            currentStats.ParentTowerName = previousStats.ParentTowerName;          
            currentStats.GradeCount = previousStats.GradeCount + 1;
            currentStats.Level = previousStats.Level;
            currentStats.Exp = previousStats.Exp;
            Debug.Log(currentStats.GradeCount);

            for (int i = 0; i < newStats.SpecialList.Length; i++)
                currentStats.SpecialList[i] = U.Instantiate(newStats.SpecialList[i]);           

            for (int i = 1; i < currentStats.Level; i++)
                IncreaseStatsPerLevel();         
            OnStatsChanged();
        }

        private void IncreaseStatsPerLevel()
        {
            currentStats.Level++;
            currentStats.Damage.Value    += Mathf.FloorToInt(QoL.GetPercentOfValue(4f, baseStats.Damage.Value));
            currentStats.AttackSpeed     -= QoL.GetPercentOfValue(1.2f, baseStats.AttackSpeed);
            currentStats.CritChance      += QoL.GetPercentOfValue(0.2f, baseStats.CritChance);
            currentStats.SpellCritChance += QoL.GetPercentOfValue(0.2f, baseStats.SpellCritChance);

            tower.SpecialSystem.IncreaseStatsPerLevel();
            var effect = U.Instantiate(GM.I.LevelUpEffect, tower.transform.position, Quaternion.identity);
            U.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);    
        }

        public void AddExp(int amount)
        {
            currentStats.Exp += amount;

            for (int i = currentStats.Level; i < 25; i++)
                if (currentStats.Exp >= GM.ExpToLevelUp[currentStats.Level] && currentStats.Level < 25)
                    IncreaseStatsPerLevel();      
            OnStatsChanged();                       
        }

        public void OnStatsChanged() => StatsChanged?.Invoke(this, new EventArgs());            
    }
}