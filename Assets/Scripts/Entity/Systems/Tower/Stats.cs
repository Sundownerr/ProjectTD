using System;
using Game.Systems;
using Game.Tower.Data;
using UnityEngine;

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

            CurrentStats = UnityEngine.Object.Instantiate(CurrentStats);
            CurrentStats.SetId();

            for (int i = 0; i < CurrentStats.AbilityList.Count; i++)
            {
                CurrentStats.AbilityList[i] = UnityEngine.Object.Instantiate(CurrentStats.AbilityList[i]);

                for (int j = 0; j < CurrentStats.AbilityList[i].EffectList.Count; j++)
                    CurrentStats.AbilityList[i].EffectList[j] = UnityEngine.Object.Instantiate(CurrentStats.AbilityList[i].EffectList[j]);

                CurrentStats.AbilityList[i].SetOwner(tower);
            }

            BaseStats = UnityEngine.Object.Instantiate(CurrentStats);

            StatsChanged?.Invoke(this, new EventArgs());
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

            StatsChanged?.Invoke(this, new EventArgs());
        }

        private void UpgradeSpecial(TowerData newBaseStats)
        {
            for (int i = 0; i < newBaseStats.SpecialList.Length; i++)
                CurrentStats.SpecialList[i] = UnityEngine.Object.Instantiate(CurrentStats.SpecialList[i]);
        }

        private void IncreaseStatsPerLevel()
        {
            CurrentStats.Level++;
            CurrentStats.Damage.Value += Mathf.FloorToInt(ExtendedMonoBehaviour.GetPercentOfValue(4f, BaseStats.Damage.Value));
            CurrentStats.AttackSpeed -= ExtendedMonoBehaviour.GetPercentOfValue(1.2f, BaseStats.AttackSpeed);
            CurrentStats.CritChance += ExtendedMonoBehaviour.GetPercentOfValue(0.2f, BaseStats.CritChance);
            CurrentStats.SpellCritChance += ExtendedMonoBehaviour.GetPercentOfValue(0.2f, BaseStats.SpellCritChance);

            tower.SpecialSystem.IncreaseStatsPerLevel();
            StatsChanged?.Invoke(this, new EventArgs());
        }

        public void AddExp(int amount)
        {
            CurrentStats.Exp += amount;

            for (int i = CurrentStats.Level; i < 25; i++)
                if (CurrentStats.Exp >= GM.ExpToLevelUp[CurrentStats.Level] && CurrentStats.Level < 25)
                {
                    IncreaseStatsPerLevel();

                    var effect = UnityEngine.Object.Instantiate(GM.Instance.LevelUpEffect, tower.transform.position, Quaternion.identity);
                    UnityEngine.Object.Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                }
            StatsChanged?.Invoke(this, new EventArgs());
          //  UpdateUI();
        }

        // public void UpdateUI()
        // {
        //     if (GM.Instance.PlayerInputSystem.ChoosedTower == tower.gameObject)
        //         GM.Instance.TowerUISystem.UpdateValues();
        // }
    }
}