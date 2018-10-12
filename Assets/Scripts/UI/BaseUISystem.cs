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

            GM.Instance.BaseUISystem = this;

            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            ReadyButton.onClick.AddListener(CheckReady);
        }

        private void StartWave()
        {
            if (GM.Instance.CreepList.Count == 0 && !IsWaveStarted)
            {
                IsWaveStarted = true;
                StartWaveButton.gameObject.SetActive(false);               
            }
        }

        private void BuildTower()
        {
            if (GM.Instance.GridSystem.IsGridBuilded)
            {
                GM.PLAYERSTATE = GM.PLAYERSTATE_PLACINGTOWER;
            }
        }

        private void CheckReady()
        {
            IsPlayerReady = true;
            Destroy(ReadyButton.gameObject);
        }      
    }
}
