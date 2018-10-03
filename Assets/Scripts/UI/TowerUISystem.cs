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

        public TextMeshProUGUI Damage, Range, Mana, AttackSpeed, TriggerChance, SpellDamage, SpellCritChance;
        public TextMeshProUGUI TowerName, CritChance;

        public Button SellButton, UpgradeButton;

        private TowerBaseSystem choosedTowerBaseSystem;
        private GameObject choosedTower;

        private void Awake()
        {
            gameObject.SetActive(false);
            SellButton.onClick.AddListener(SellTower);
        }

        private void OnEnable()
        {
            choosedTower = GameManager.Instance.PlayerSystem.ChoosedTower;
            choosedTowerBaseSystem = choosedTower.GetComponent<TowerBaseSystem>();
            var choosedTowerStats = choosedTowerBaseSystem.Stats;

            choosedTowerBaseSystem.RangeSystem.Show(true);

            TowerName.text = choosedTowerStats.entityName;
            Damage.text = KiloFormat(choosedTowerStats.Damage);
            Range.text = KiloFormat(choosedTowerStats.Range);
            Mana.text = KiloFormat(choosedTowerStats.Mana);
            AttackSpeed.text = KiloFormat(choosedTowerStats.AttackSpeed);
            TriggerChance.text = KiloFormat(choosedTowerStats.TriggerChance) + "%";
            SpellCritChance.text = KiloFormat(choosedTowerStats.SpellCritChance) + "%";
            SpellDamage.text = KiloFormat(choosedTowerStats.SpellDamage) + "%";           
            CritChance.text = KiloFormat(choosedTowerStats.CritChance) + "%";
        }

        private void OnDisable()
        {
            if (choosedTowerBaseSystem != null)
            {
                choosedTowerBaseSystem.RangeSystem.Show(false);
            }
        }

        public IEnumerator RefreshUI()
        {
            gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            gameObject.SetActive(true);
        }

        private void SellTower()
        {
            for (int i = 0; i < GameManager.Instance.TowerList.Count; i++)
            {
                if(GameManager.Instance.TowerList[i] == choosedTower)
                {
                    choosedTower.GetComponent<TowerBaseSystem>().Sell();
                    gameObject.SetActive(false);
                }
            }
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

            return num.ToString("0.#");
        }
    }
}

