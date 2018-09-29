using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#pragma warning disable CS1591 
namespace Game.System
{
    public class TowerUISystem : MonoBehaviour
    {

        public TextMeshProUGUI DamageValue, RangeValue, ManaValue, AttackSpeedValue, TriggerChanceValue, SpellDamageValue, SpellCritChanceValue;
        public TextMeshProUGUI TowerName, CritChanceValue;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            var choosedTowerStats = GameManager.Instance.PlayerSystem.ChoosedTower.GetComponent<Tower.Tower>().TowerStats;

            DamageValue.text = KiloFormat(choosedTowerStats.damage).ToString();
            RangeValue.text = KiloFormat(choosedTowerStats.range).ToString();
            ManaValue.text = KiloFormat(choosedTowerStats.mana).ToString();
            AttackSpeedValue.text = KiloFormat(choosedTowerStats.attackSpeed).ToString();
            TriggerChanceValue.text = KiloFormat(choosedTowerStats.triggerChance).ToString() + "%";
            SpellCritChanceValue.text = KiloFormat(choosedTowerStats.spellCritChance).ToString() + "%";
            SpellDamageValue.text = KiloFormat(choosedTowerStats.spellDamage).ToString() + "%";
            TowerName.text = choosedTowerStats.entityName;
            CritChanceValue.text = KiloFormat(choosedTowerStats.critChance).ToString() + "%";
            
        }
        
        public IEnumerator RefreshUI()
        {
            gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            gameObject.SetActive(true);
        }

        private string KiloFormat(float num)
        {
            if (num >= 1000000000)
                return (num / 1000000000).ToString("#.0" + "B");

            if (num >= 1000000)
                return (num / 1000000).ToString("#" + "M");

            if (num >= 100000)
                return (num / 1000).ToString("#.0" + "K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#" + "K");

            if (num >= 1000)
                return (num / 1000).ToString("0.#" + "K");

            return num.ToString("#,0");
        }
    }
}

