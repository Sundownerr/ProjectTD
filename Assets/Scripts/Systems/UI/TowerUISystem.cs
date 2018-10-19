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
        public TextMeshProUGUI TowerName, CritChance, Level;
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
            UpdateValues();
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

        public void UpdateValues()
        {
            choosedTower = GM.Instance.PlayerInputSystem.ChoosedTower;
            var choosedTowerBaseSystem = choosedTower.GetComponent<TowerBaseSystem>();
            var choosedTowerStats = choosedTowerBaseSystem.Stats;

            TowerName.text = choosedTowerStats.EntityName;
            Level.text = KiloFormat(choosedTowerStats.Level);
            Damage.text = KiloFormat(choosedTowerStats.Damage);
            Range.text = KiloFormat(choosedTowerStats.Range);
            Mana.text = KiloFormat(choosedTowerStats.Mana);
            AttackSpeed.text = KiloFormat(choosedTowerStats.AttackSpeed);
            TriggerChance.text = KiloFormat(choosedTowerStats.TriggerChance) + "%";
            SpellCritChance.text = KiloFormat(choosedTowerStats.SpellCritChance) + "%";
            SpellDamage.text = KiloFormat(choosedTowerStats.SpellDamage) + "%";
            CritChance.text = KiloFormat(choosedTowerStats.CritChance) + "%";
        }       
    }
}

