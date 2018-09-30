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
            var choosedTowerStats = choosedTowerBaseSystem.TowerStats;

            choosedTowerBaseSystem.TowerRange.Show(true);

            TowerName.text = choosedTowerStats.entityName;
            Damage.text = KiloFormat(choosedTowerStats.damage);
            Range.text = KiloFormat(choosedTowerStats.range);
            Mana.text = KiloFormat(choosedTowerStats.mana);
            AttackSpeed.text = KiloFormat(choosedTowerStats.attackSpeed);
            TriggerChance.text = KiloFormat(choosedTowerStats.triggerChance) + "%";
            SpellCritChance.text = KiloFormat(choosedTowerStats.spellCritChance) + "%";
            SpellDamage.text = KiloFormat(choosedTowerStats.spellDamage) + "%";           
            CritChance.text = KiloFormat(choosedTowerStats.critChance) + "%";
        }

        private void OnDisable()
        {
            if (choosedTowerBaseSystem != null)
            {
                choosedTowerBaseSystem.TowerRange.Show(false);
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
                    GameManager.Instance.TowerList[i].GetComponent<TowerCombatSystem>().bulletPool.DestroyPool();
                    Destroy(GameManager.Instance.TowerList[i]);
                    GameManager.Instance.TowerList.RemoveAt(i);
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

            return num.ToString("#,0");
        }
    }
}

