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

        public void Set()
        {
            currentStats = U.Instantiate(currentStats);
            currentStats.SetId();
            currentStats.SetData();

            for (int i = 0; i < currentStats.AbilityList.Count; i++)
            {
                currentStats.AbilityList[i] = U.Instantiate(currentStats.AbilityList[i]);

                for (int j = 0; j < currentStats.AbilityList[i].EffectList.Count; j++)
                    currentStats.AbilityList[i].EffectList[j] = U.Instantiate(currentStats.AbilityList[i].EffectList[j]);

                currentStats.AbilityList[i].SetOwner(tower);
            }

            baseStats = U.Instantiate(currentStats);
            baseStats.IsInstanced = true;
            OnStatsChanged();
        }

        public void Upgrade(TowerData currentStats, TowerData newBaseStats)
        {   
            this.currentStats = U.Instantiate(newBaseStats);
            this.currentStats.SetData();

            if(currentStats.ParentTowerName == null)
                this.currentStats.ParentTowerName = currentStats.Name;
            else
                this.currentStats.ParentTowerName = currentStats.ParentTowerName;
            
            this.currentStats.GradeCount = currentStats.GradeCount + 1;
            this.currentStats.Level = currentStats.Level;
            this.currentStats.Exp = currentStats.Exp;

            for (int i = 0; i < newBaseStats.SpecialList.Length; i++)
                this.currentStats.SpecialList[i] = U.Instantiate(newBaseStats.SpecialList[i]);

            baseStats = U.Instantiate(newBaseStats);
            baseStats.IsInstanced = true;

            for (int i = 1; i < this.currentStats.Level; i++)
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
            OnStatsChanged();
        }

        public void AddExp(int amount)
        {
            currentStats.Exp += amount;

            for (int i = currentStats.Level; i < 25; i++)
                if (currentStats.Exp >= GM.ExpToLevelUp[currentStats.Level] && currentStats.Level < 25)
                {
                    IncreaseStatsPerLevel();

                    var effect = U.Instantiate(GM.I.LevelUpEffect, tower.transform.position, Quaternion.identity);
                    U.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                }
            OnStatsChanged();
        }

        public void OnStatsChanged() => StatsChanged?.Invoke(this, new EventArgs());            
    }
}