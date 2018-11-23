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

            GM.I.TowerUISystem = this;
            
            SellButton.onClick.AddListener(Sell);
            UpgradeButton.onClick.AddListener(Upgrade);         
        }

        private void Start()
        {
            GM.I.PlayerInputSystem.MouseOnTower += UpdateValues;   
            gameObject.SetActive(false);         
        }

       
        private void Sell()
        {
            Selling?.Invoke(this, new EventArgs());
            gameObject.SetActive(false);
        } 
        
        private void Upgrade() 
        {
            Upgrading?.Invoke(this, new EventArgs());
            UpdateValues(this, new EventArgs());
        } 
        
        public void UpdateValues(object sender, EventArgs e)
        {
            choosedTower = GM.I.PlayerInputSystem.ChoosedTower;  
            
            TowerName.text = choosedTower.Stats.Name;
            Level.text              = QoL.KiloFormat(choosedTower.Stats.Level);
            Damage.text             = QoL.KiloFormat(choosedTower.Stats.Damage.Value);
            Range.text              = QoL.KiloFormat(choosedTower.Stats.Range);
            Mana.text               = QoL.KiloFormat(choosedTower.Stats.Mana);
            AttackSpeed.text        = QoL.KiloFormat(choosedTower.Stats.AttackSpeed);
            TriggerChance.text      = QoL.KiloFormat(choosedTower.Stats.TriggerChance) + "%";
            SpellCritChance.text    = QoL.KiloFormat(choosedTower.Stats.SpellCritChance) + "%";
            SpellDamage.text        = QoL.KiloFormat(choosedTower.Stats.SpellDamage) + "%";
            CritChance.text         = QoL.KiloFormat(choosedTower.Stats.CritChance) + "%";
           
        }            

        public void ActivateUpgradeButton(bool activate) => UpgradeButton.gameObject.SetActive(activate);
    }
}

