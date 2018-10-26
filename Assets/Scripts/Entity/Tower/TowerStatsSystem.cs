using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data.Entity.Tower;
using Game.System;

namespace Game.Tower
{
    public class TowerStatsSystem : ExtendedMonoBehaviour
    {
        public TowerData Stats;

        private TowerBaseSystem ownerTower;
        public TowerData BaseStats;
        private List<TowerData> gradeList;
        private int gradeCount;
        private StateMachine state;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            Stats = Instantiate(Stats);
            BaseStats = Instantiate(Stats);
            gradeList = BaseStats.GradeList;

            ownerTower = GetComponent<TowerBaseSystem>();

            for (int i = 0; i < Stats.AbilityList.Count; i++)
            {
                Stats.AbilityList[i] = Instantiate(Stats.AbilityList[i]);

                for (int j = 0; j < Stats.AbilityList[i].EffectList.Count; j++)
                {
                    Stats.AbilityList[i].EffectList[j] = Instantiate(Stats.AbilityList[i].EffectList[j]);
                }

                Stats.AbilityList[i].SetOwnerTower(ownerTower);
            }
        }

        public void Upgrade(TowerData currentBase, TowerData current, TowerData newBase)
        {
            Stats = Instantiate(newBase);
            Stats.Level = current.Level;
            Stats.Exp = current.Exp;

            for (int i = 1; i < Stats.Level; i++)
            {
                IncreaseStatsPerLevel();
            }

            BaseStats = Instantiate(newBase);
        }

        private void IncreaseStatsPerLevel()
        {
            Stats.Damage += Mathf.FloorToInt(GetPercentOfValue(4f, BaseStats.Damage));
            Stats.AttackSpeed -= GetPercentOfValue(1.2f, BaseStats.AttackSpeed);
            Stats.CritChance += GetPercentOfValue(0.2f, BaseStats.CritChance);
            Stats.SpellCritChance += GetPercentOfValue(0.2f, BaseStats.SpellCritChance);          
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

                    var effect = Instantiate(GM.Instance.LevelUpEffect, transform.position, Quaternion.identity);
                    Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                }
            }
            UpdateUI();
        }


        public void UpdateUI()
        {
            if (GM.Instance.PlayerInputSystem.ChoosedTower == gameObject)
            {
                GM.Instance.TowerUISystem.UpdateValues();
            }
        }
    }
}