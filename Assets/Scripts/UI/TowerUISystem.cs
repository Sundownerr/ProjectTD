using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Tower;
#pragma warning disable CS1591 
namespace Game.System
{
    public class TowerUISystem : MonoBehaviour
    {

        public TextMeshProUGUI DamageValue, RangeValue, ManaValue, AttackSpeedValue, TriggerChanceValue, SpellDamageValue, SpellCritChanceValue;
        public TextMeshProUGUI TowerName, CritChanceValue;

        public TowerBaseSystem choosedtowerBaseSystem;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            var choosedTower = GameManager.Instance.PlayerSystem.ChoosedTower;
            choosedtowerBaseSystem = choosedTower.GetComponent<TowerBaseSystem>();
            var choosedTowerStats = choosedtowerBaseSystem.TowerStats;

            choosedtowerBaseSystem.TowerRange.Show(true);

            TowerName.text = choosedTowerStats.entityName;
            DamageValue.text = KiloFormat(choosedTowerStats.damage);
            RangeValue.text = KiloFormat(choosedTowerStats.range);
            ManaValue.text = KiloFormat(choosedTowerStats.mana);
            AttackSpeedValue.text = KiloFormat(choosedTowerStats.attackSpeed);
            TriggerChanceValue.text = KiloFormat(choosedTowerStats.triggerChance) + "%";
            SpellCritChanceValue.text = KiloFormat(choosedTowerStats.spellCritChance) + "%";
            SpellDamageValue.text = KiloFormat(choosedTowerStats.spellDamage) + "%";           
            CritChanceValue.text = KiloFormat(choosedTowerStats.critChance) + "%";
        }

        private void OnDisable()
        {
            if (choosedtowerBaseSystem != null)
            {
                choosedtowerBaseSystem.TowerRange.Show(false);
            }
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

