using UnityEngine.UI;
using TMPro;

namespace Game.System
{
    public class BaseUISystem : ExtendedMonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton, LearnForceButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;

        public TextMeshProUGUI Gold, MagicCrystals, TowerLimit;
        private bool isForceMenuShowed;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }         
            
            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            LearnForceButton.onClick.AddListener(LearnForce);

            UpdateResourceValues();

            GM.Instance.BaseUISystem = this;
        }

        public void UpdateResourceValues()
        {
            Gold.text = KiloFormat(GM.Instance.PlayerData.Gold);
            MagicCrystals.text = KiloFormat(GM.Instance.PlayerData.MagicCrystals);
            TowerLimit.text = GM.Instance.PlayerData.CurrentTowerLimit + "/" + GM.Instance.PlayerData.MaxTowerLimit;
        }

        private void StartWave()
        {
            IsWaveStarted = true;
        }

        private void LearnForce()
        {
            if (!isForceMenuShowed)
            {
                GM.Instance.ForceUISystem.ShowElementButtons(true);
                isForceMenuShowed = true;
            }
            else
            {
                GM.Instance.ForceUISystem.ShowElementButtons(false);
                isForceMenuShowed = false;
            }        
        }

        private void BuildTower()
        {
            if (GM.Instance.GridSystem.IsGridBuilded)
            {
                if (GM.PLAYERSTATE != GM.PLACING_TOWER)
                {
                    GM.PLAYERSTATE = GM.PREPARE_PLACING_TOWER;

                    if (isForceMenuShowed)
                    {
                        GM.Instance.ForceUISystem.ShowElementButtons(false);
                        isForceMenuShowed = false;
                    }
                }
            }
        }
    }
}
