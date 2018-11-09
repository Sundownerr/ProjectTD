using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Tower;
using System;

namespace Game.Systems
{
    public class TowerUISystem : ExtendedMonoBehaviour
    {
        public TextMeshProUGUI Damage, Range, Mana, AttackSpeed, TriggerChance, SpellDamage, SpellCritChance;
        public TextMeshProUGUI TowerName, CritChance, Level;
        public Button SellButton, UpgradeButton;
        public event EventHandler Selling = delegate{};
        public event EventHandler Upgrading = delegate{};

        private TowerSystem choosedTower;

        protected override void Awake()
        {
            base.Awake();

            GM.Instance.TowerUISystem = this;
            
            SellButton.onClick.AddListener(Sell);
            UpgradeButton.onClick.AddListener(Upgrade);         
        }

        private void Start()
        {
            GM.Instance.PlayerInputSystem.MouseOnTower += UpdateValues;   
            gameObject.SetActive(false);         
        }

        private void OnDisable()
        {
            if(GM.Instance.PlayerInputSystem?.ChoosedTower != null)
                GM.Instance.PlayerInputSystem.ChoosedTower.StatsSystem.StatsChanged -= UpdateValues;
        }

        private void Sell()
        {
            Selling?.Invoke(this, new EventArgs());
            gameObject.SetActive(false);
        } 
        
        private void Upgrade() => Upgrading?.Invoke(this, new EventArgs());
        
        private void UpdateValues(object sender, EventArgs e)
        {
            choosedTower = GM.Instance.PlayerInputSystem.ChoosedTower;  
            choosedTower.StatsSystem.StatsChanged += UpdateValues;

            TowerName.text = choosedTower.Stats.Name;
            Level.text              = KiloFormat(choosedTower.Stats.Level);
            Damage.text             = KiloFormat(choosedTower.Stats.Damage.Value);
            Range.text              = KiloFormat(choosedTower.Stats.Range);
            Mana.text               = KiloFormat(choosedTower.Stats.Mana);
            AttackSpeed.text        = KiloFormat(choosedTower.Stats.AttackSpeed);
            TriggerChance.text      = KiloFormat(choosedTower.Stats.TriggerChance) + "%";
            SpellCritChance.text    = KiloFormat(choosedTower.Stats.SpellCritChance) + "%";
            SpellDamage.text        = KiloFormat(choosedTower.Stats.SpellDamage) + "%";
            CritChance.text         = KiloFormat(choosedTower.Stats.CritChance) + "%";
      
            UpgradeButton.gameObject.SetActive(choosedTower.Stats.GradeList.Count > 0);          
        }            
    }
}

