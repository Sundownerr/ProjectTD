using System;
using Game.Systems;
using Game.Tower.Data;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Tower.System
{
    [Serializable]
    public class Stats : ITowerSystem
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
       
            baseStats = stats;
           
            ChangedStats();
        }

        public void Upgrade(TowerSystem previousTower, TowerData newStats)
        {                  
            Set(newStats);         
            currentStats.Id                 = previousTower.Stats.Id;
            currentStats.GradeCount         = previousTower.Stats.GradeCount + 1;
            currentStats.Exp                = previousTower.Stats.Exp;
            currentStats.AttackSpeedModifier = previousTower.Stats.AttackSpeedModifier;
            tower.OcuppiedCell              = previousTower.OcuppiedCell;   

            for (int i = 0; i < previousTower.Stats.Level; i++)
                IncreaseStatsPerLevel();                    
                  
            ChangedStats();
        }

        private void IncreaseStatsPerLevel()
        {
            currentStats.Level++;
            currentStats.Damage.Value    += Mathf.FloorToInt(QoL.GetPercentOfValue(4f, baseStats.Damage.Value));
            currentStats.AttackSpeed     -= QoL.GetPercentOfValue(1.2f, baseStats.AttackSpeed);
            currentStats.CritChance      += QoL.GetPercentOfValue(0.2f, baseStats.CritChance);
            currentStats.SpellCritChance += QoL.GetPercentOfValue(0.2f, baseStats.SpellCritChance);

            tower.TraitControlSystem.IncreaseStatsPerLevel();
            var effect = U.Instantiate(GM.I.LevelUpEffect, tower.Prefab.transform.position, Quaternion.identity);
            U.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);              
        }

        public void AddExp(int amount)
        {
            currentStats.Exp += amount;

            for (int i = currentStats.Level; i < 25; i++)
                if (currentStats.Exp >= GM.ExpToLevelUp[currentStats.Level] && currentStats.Level < 25)
                    IncreaseStatsPerLevel();      
            
            if (GM.I.PlayerInputSystem.ChoosedTower == tower)
                ChangedStats();    
        }

        public void ChangedStats() => StatsChanged?.Invoke(this, new EventArgs());                 
    }
}