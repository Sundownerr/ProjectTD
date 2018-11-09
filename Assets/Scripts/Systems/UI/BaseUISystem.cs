using UnityEngine.UI;
using TMPro;
using System;

namespace Game.Systems
{
    public class BaseUISystem : ExtendedMonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton, LearnForceButton, GetTowerButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;
        public TextMeshProUGUI Gold, MagicCrystals, TowerLimit;
   
        protected override void Awake()
        {
            base.Awake();
            
            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            LearnForceButton.onClick.AddListener(LearnForce);
            GetTowerButton.onClick.AddListener(GetTower);

            UpdateUI(this, new EventArgs());

            GM.Instance.BaseUISystem = this;   
        }

        public void Start()
        {
            GM.Instance.ResourceSystem.ResourcesChanged += UpdateUI;
        }

        public void UpdateUI(object sender, EventArgs e)
        {
            Gold.text           = KiloFormat(GM.Instance.PlayerData.Gold);
            MagicCrystals.text  = KiloFormat(GM.Instance.PlayerData.MagicCrystals);
            TowerLimit.text     = GM.Instance.PlayerData.CurrentTowerLimit + "/" + GM.Instance.PlayerData.MaxTowerLimit;
        }

        private void StartWave()
        {
            IsWaveStarted = GM.Instance.WaveSystem.WaveNumber <= GM.Instance.WaveAmount;
        }

        private void LearnForce()
        {
            if (GM.Instance.ElementUISystem.gameObject.activeSelf)
                GM.Instance.ElementUISystem.gameObject.SetActive(false);
            else
            {
                GM.Instance.ElementUISystem.gameObject.SetActive(true);
                GM.Instance.BuildUISystem.gameObject.SetActive(false);
            }         
        }

        private void GetTower()
        {
            GM.Instance.TowerCreatingSystem.CreateRandomTower();        
        }

        private void BuildTower()
        {
            if (GM.Instance.BuildUISystem.gameObject.activeSelf)
                GM.Instance.BuildUISystem.gameObject.SetActive(false);
            else
            {
                GM.Instance.BuildUISystem.gameObject.SetActive(true);
                GM.Instance.ElementUISystem.gameObject.SetActive(false);
            }         
        }
    }
}
