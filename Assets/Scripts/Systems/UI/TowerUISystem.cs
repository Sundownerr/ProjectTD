using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Tower;

namespace Game.Systems
{
    public class TowerUISystem : ExtendedMonoBehaviour
    {
        public bool IsSellig { get => isSellig; set => isSellig = value; }
        public bool IsUpgrading { get => isUpgrading; set => isUpgrading = value; }
        
        public TextMeshProUGUI Damage, Range, Mana, AttackSpeed, TriggerChance, SpellDamage, SpellCritChance;
        public TextMeshProUGUI TowerName, CritChance, Level;
        public Button SellButton, UpgradeButton;

        private GameObject choosedTower;
        private bool isSellig, isUpgrading;  

        protected override void Awake()
        {
            base.Awake();

            GM.Instance.TowerUISystem = this;
            gameObject.SetActive(false);
            SellButton.onClick.AddListener(Sell);
            UpgradeButton.onClick.AddListener(Upgrade);
        }

        private void OnEnable() => UpdateValues();
    
        public IEnumerator RefreshUI()
        {
            gameObject.SetActive(false);
            yield return new WaitForFixedUpdate();
            gameObject.SetActive(true);
        }

        private void Sell() => IsSellig = true;

        private void Upgrade() => IsUpgrading = true;

        public void UpdateValues()
        {
            choosedTower = GM.Instance.PlayerInputSystem.ChoosedTower;
            var choosedTowerStats = choosedTower.GetComponent<TowerSystem>().GetStats();

            TowerName.text = choosedTowerStats.Name;
            Level.text              = KiloFormat(choosedTowerStats.Level);
            Damage.text             = KiloFormat(choosedTowerStats.Damage.Value);
            Range.text              = KiloFormat(choosedTowerStats.Range);
            Mana.text               = KiloFormat(choosedTowerStats.Mana);
            AttackSpeed.text        = KiloFormat(choosedTowerStats.AttackSpeed);
            TriggerChance.text      = KiloFormat(choosedTowerStats.TriggerChance) + "%";
            SpellCritChance.text    = KiloFormat(choosedTowerStats.SpellCritChance) + "%";
            SpellDamage.text        = KiloFormat(choosedTowerStats.SpellDamage) + "%";
            CritChance.text         = KiloFormat(choosedTowerStats.CritChance) + "%";

            var isHaveUpgrade =
              choosedTowerStats.GradeList.Count > 0;          

            if (isHaveUpgrade)
                UpgradeButton.gameObject.SetActive(true);
            else
                UpgradeButton.gameObject.SetActive(false);
        }       
    }
}

