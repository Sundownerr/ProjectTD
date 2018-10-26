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

        private void IncreaseStatsPerLevel()
        {
            Stats.Damage = BaseStats.Damage + Mathf.FloorToInt(GetPercentOfValue(4f, BaseStats.Damage));
            Stats.AttackSpeed = BaseStats.AttackSpeed - GetPercentOfValue(1.2f, BaseStats.AttackSpeed);
            Stats.CritChance = BaseStats.CritChance + GetPercentOfValue(0.2f, BaseStats.CritChance);
            Stats.SpellCritChance = BaseStats.SpellCritChance + GetPercentOfValue(0.2f, BaseStats.SpellCritChance);

            UpdateUI();
        }

        public void Upgrade(TowerData currentBase, TowerData newBase)
        {
            Stats.EntityName = newBase.EntityName;
            Stats.Image = newBase.Image;
            Stats.EntityDescription = newBase.EntityDescription;
            Stats.Damage = newBase.Damage + (Stats.Damage - currentBase.Damage);
            Stats.Range = newBase.Range;
            Stats.AttackSpeed = newBase.AttackSpeed - (Stats.AttackSpeed - currentBase.AttackSpeed);
            Stats.CritChance = newBase.CritChance + (Stats.CritChance - currentBase.CritChance);
            Stats.CritMultiplier = newBase.CritMultiplier + (Stats.CritMultiplier - currentBase.CritMultiplier);
            Stats.MulticritCount = newBase.MulticritCount + (Stats.MulticritCount - currentBase.MulticritCount);
            Stats.Mana = newBase.Mana + (Stats.Mana - currentBase.Mana);
            Stats.ManaRegen = newBase.ManaRegen + (Stats.ManaRegen - currentBase.ManaRegen);
            Stats.SpellDamage = newBase.SpellDamage + (Stats.SpellDamage - currentBase.SpellDamage);
            Stats.SpellCritChance = newBase.SpellCritChance + (Stats.SpellCritChance - currentBase.SpellCritChance);
            Stats.TriggerChance = newBase.TriggerChance + (Stats.TriggerChance - currentBase.TriggerChance);
            Stats.BuffDuration = newBase.BuffDuration + (Stats.BuffDuration - currentBase.BuffDuration);
            Stats.DebuffDuration = newBase.DebuffDuration + (Stats.DebuffDuration - currentBase.DebuffDuration);
            Stats.ExpRatio = newBase.ExpRatio + (Stats.ExpRatio - currentBase.ExpRatio);
            Stats.ItemDropRatio = newBase.ItemDropRatio + (Stats.ItemDropRatio - currentBase.ItemDropRatio);
            Stats.ItemQuialityRatio = newBase.ItemQuialityRatio + (Stats.ItemQuialityRatio - currentBase.ItemQuialityRatio);
            Stats.GoldRatio = newBase.GoldRatio + (Stats.GoldRatio - currentBase.GoldRatio);
    

            BaseStats = newBase;
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
                    Destroy(effect, effect.GetComponent<ParticleSystem>().main.startLifetime.constant);
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