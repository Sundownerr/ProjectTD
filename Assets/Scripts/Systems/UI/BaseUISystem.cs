﻿using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine;

namespace Game.Systems
{
    public class BaseUISystem : ExtendedMonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton, LearnForceButton, GetTowerButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;
        public TextMeshProUGUI Gold, MagicCrystals, TowerLimit;
        public event EventHandler WaveStarted = delegate{};
   
        protected override void Awake()
        {
            base.Awake();
            
            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            LearnForceButton.onClick.AddListener(LearnForce);
            GetTowerButton.onClick.AddListener(GetTower);

            

            GM.I.BaseUISystem = this;   
        }

        public void Start()
        {
            GM.I.ResourceSystem.ResourcesChanged += UpdateUI;
            UpdateUI(this, new EventArgs());
        }

        public void UpdateUI(object sender, EventArgs e)
        {
            Gold.text           = QoL.KiloFormat(GM.I.PlayerData.Gold);
            MagicCrystals.text  = QoL.KiloFormat(GM.I.PlayerData.MagicCrystals);
            TowerLimit.text     = GM.I.PlayerData.CurrentTowerLimit + "/" + GM.I.PlayerData.MaxTowerLimit;
        }

        private void StartWave()
        {
            // IsWaveStarted = GM.I.WaveSystem.WaveNumber <= GM.I.WaveAmount;
            if (GM.I.WaveSystem.WaveNumber <= GM.I.WaveAmount)
            {
                StartWaveButton.gameObject.SetActive(false);
                WaveStarted?.Invoke(this, new EventArgs());
            }
        }

        private void LearnForce()
        {
            if (GM.I.ElementUISystem.gameObject.activeSelf)
                GM.I.ElementUISystem.gameObject.SetActive(false);
            else
            {
                GM.I.ElementUISystem.gameObject.SetActive(true);
                GM.I.BuildUISystem.gameObject.SetActive(false);
            }         
        }

        private void GetTower()
        {
            GM.I.TowerCreatingSystem.CreateRandomTower();        
        }

        private void BuildTower()
        {
            if (GM.I.BuildUISystem.gameObject.activeSelf)
                GM.I.BuildUISystem.gameObject.SetActive(false);
            else
            {
                GM.I.BuildUISystem.gameObject.SetActive(true);
                GM.I.ElementUISystem.gameObject.SetActive(false);
            }         
        }
    }
}
