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
        private TowerData baseStats;
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
            baseStats = Instantiate(Stats);
            gradeList = baseStats.GradeList;

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

        private void IncreaseStats()
        {
            Stats.Damage = baseStats.Damage + Mathf.FloorToInt(GetPercentOfValue(4f, baseStats.Damage));
            Stats.AttackSpeed = baseStats.AttackSpeed + GetPercentOfValue(1.2f, baseStats.AttackSpeed);
            Stats.CritChance = baseStats.CritChance + GetPercentOfValue(0.2f, baseStats.CritChance);
            Stats.SpellCritChance = baseStats.SpellCritChance + GetPercentOfValue(0.2f, baseStats.SpellCritChance);

            UpdateUI();
        }


        public void Upgrade()
        {
            if (gradeCount < gradeList.Count)
            {
                baseStats = Instantiate(gradeList[gradeCount]);

                UpdateUI();

                gradeCount++;
            }
        }

        public void AddExp(int amount)
        {
            Stats.Exp += amount;

            for (int i = Stats.Level; i < 25; i++)
            {
                if (Stats.Exp >= GM.ExpToLevelUp[Stats.Level - 1] && Stats.Level < 25)
                {
                    IncreaseStats();
                    Stats.Level++;

                    var effect = Instantiate(GM.Instance.LevelUpEffect, transform.position, Quaternion.identity);
                    Destroy(effect, effect.GetComponent<ParticleSystem>().main.startLifetime.constant);
                }
            }
            UpdateUI();
        }


        private void UpdateUI()
        {
            if (GM.Instance.PlayerInputSystem.ChoosedTower == gameObject)
            {
                GM.Instance.TowerUISystem.UpdateValues();
            }
        }
    }
}