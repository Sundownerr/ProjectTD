using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Tower;

namespace Game.System
{
    public class TowerUISystem : ExtendedMonoBehaviour
    {

        public TextMeshProUGUI Damage, Range, Mana, AttackSpeed, TriggerChance, SpellDamage, SpellCritChance;
        public TextMeshProUGUI TowerName, CritChance;
        public bool IsSelligTower;

        public Button SellButton, UpgradeButton;

        private GameObject choosedTower;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            } 

            GM.Instance.TowerUISystem = this;
            gameObject.SetActive(false);
            SellButton.onClick.AddListener(SellTower);
        }

        private void OnEnable()
        {
            choosedTower = GM.Instance.PlayerInputSystem.ChoosedTower;
            var choosedTowerBaseSystem = choosedTower.GetComponent<TowerBaseSystem>();
            var choosedTowerStats = choosedTowerBaseSystem.Stats;         

            TowerName.text = choosedTowerStats.EntityName;
            Damage.text = KiloFormat(choosedTowerStats.Damage);
            Range.text = KiloFormat(choosedTowerStats.Range);
            Mana.text = KiloFormat(choosedTowerStats.Mana);
            AttackSpeed.text = KiloFormat(choosedTowerStats.AttackSpeed);
            TriggerChance.text = KiloFormat(choosedTowerStats.TriggerChance) + "%";
            SpellCritChance.text = KiloFormat(choosedTowerStats.SpellCritChance) + "%";
            SpellDamage.text = KiloFormat(choosedTowerStats.SpellDamage) + "%";           
            CritChance.text = KiloFormat(choosedTowerStats.CritChance) + "%";
        }

        public IEnumerator RefreshUI()
        {
            gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            gameObject.SetActive(true);
        }

        private void SellTower()
        {
            IsSelligTower = true;
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

