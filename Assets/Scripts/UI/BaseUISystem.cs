using UnityEngine.UI;

namespace Game.System
{
    public class BaseUISystem : ExtendedMonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton, ReadyButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GameManager.Instance.BaseUISystem = this;

            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            ReadyButton.onClick.AddListener(CheckReady);
        }

        private void StartWave()
        {
            if (GameManager.Instance.CreepList.Count == 0 && !IsWaveStarted)
            {
                IsWaveStarted = true;
                StartWaveButton.gameObject.SetActive(false);               
            }
        }

        private void BuildTower()
        {
            if (GameManager.Instance.GridSystem.IsGridBuilded)
            {
                GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
            }
        }

        private void CheckReady()
        {
            IsPlayerReady = true;
            Destroy(ReadyButton.gameObject);
        }      
    }
}
