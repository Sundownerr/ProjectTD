using UnityEngine.UI;
using TMPro;

namespace Game.System
{
    public class BaseUISystem : ExtendedMonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;

        public TextMeshProUGUI Gold, MagicCrystals, TowerLimit;
        

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.BaseUISystem = this;
            
            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);

            UpdateResourceValues();
        }

        public void UpdateResourceValues()
        {
            Gold.text = GM.KiloFormat(GM.Instance.PlayerData.Gold);
            MagicCrystals.text = GM.KiloFormat(GM.Instance.PlayerData.MagicCrystals);
            TowerLimit.text = GM.KiloFormat(GM.Instance.PlayerData.CurrentTowerLimit) + "/" + GM.Instance.PlayerData.MaxTowerLimit;
        }

        private void StartWave()
        {
            IsWaveStarted = true;
        }

        private void BuildTower()
        {
            if (GM.Instance.GridSystem.IsGridBuilded)
            {
                if (GM.PLAYERSTATE != GM.PLACING_TOWER)
                {
                    GM.PLAYERSTATE = GM.PREPARE_PLACING_TOWER;
                }
            }
        }
    }
}
