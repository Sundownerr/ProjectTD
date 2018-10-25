using UnityEngine.UI;
using TMPro;

namespace Game.System
{
    public class BaseUISystem : ExtendedMonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton, LearnForceButton, GetTowerButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;
        public TextMeshProUGUI Gold, MagicCrystals, TowerLimit;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }         
            
            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            LearnForceButton.onClick.AddListener(LearnForce);
            GetTowerButton.onClick.AddListener(GetTower);

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
            if (!GM.Instance.ElementUISystem.gameObject.activeSelf)
            {
                GM.Instance.ElementUISystem.gameObject.SetActive(true);

                GM.Instance.BuildUISystem.gameObject.SetActive(false);
            }
            else
            {
                GM.Instance.ElementUISystem.gameObject.SetActive(false);
            }        
        }

        private void GetTower()
        {
            GM.Instance.TowerCreatingSystem.CreateRandomTower();
        
        }

        private void BuildTower()
        {
            if (!GM.Instance.BuildUISystem.gameObject.activeSelf)
            {
                GM.Instance.BuildUISystem.gameObject.SetActive(true);

                GM.Instance.ElementUISystem.gameObject.SetActive(false);
            }
            else
            {
                GM.Instance.BuildUISystem.gameObject.SetActive(false);
            }
            //if (GM.Instance.GridSystem.IsGridBuilded)
            //{
            //    if (GM.PLAYERSTATE != GM.PLACING_TOWER)
            //    {
            //        GM.PLAYERSTATE = GM.PREPARE_PLACING_TOWER;

            //        if (GM.Instance.ElementUISystem.gameObject.activeSelf)
            //        {
            //            GM.Instance.ElementUISystem.gameObject.SetActive(false);
            //        }
            //    }
            //}
        }
    }
}
