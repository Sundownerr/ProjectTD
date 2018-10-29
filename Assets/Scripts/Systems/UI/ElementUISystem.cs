using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Game.System
{
    public class ElementUISystem : ExtendedMonoBehaviour
    {
        public Button Astral, Darkness, Ice, Iron, Storm, Nature, Fire;
        public TextMeshProUGUI AstralLevel, DarknessLevel, IceLevel, IronLevel, StormLevel, NatureLevel, FireLevel;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
                CachedTransform = transform;

            Astral.onClick.AddListener(LearnAstral);
            Darkness.onClick.AddListener(LearnDarkness);
            Ice.onClick.AddListener(LearnIce);
            Iron.onClick.AddListener(LearnIron);
            Storm.onClick.AddListener(LearnStorm);
            Nature.onClick.AddListener(LearnNature);
            Fire.onClick.AddListener(LearnFire);

            GM.Instance.ElementUISystem = this;

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            AstralLevel.text = GM.Instance.PlayerData.ElementLevelList[0].ToString();
            DarknessLevel.text = GM.Instance.PlayerData.ElementLevelList[1].ToString();
            IceLevel.text = GM.Instance.PlayerData.ElementLevelList[2].ToString();
            IronLevel.text = GM.Instance.PlayerData.ElementLevelList[3].ToString();
            StormLevel.text = GM.Instance.PlayerData.ElementLevelList[4].ToString();
            NatureLevel.text = GM.Instance.PlayerData.ElementLevelList[5].ToString();
            FireLevel.text = GM.Instance.PlayerData.ElementLevelList[6].ToString();
        }

        private void LearnAstral()
        {
            GM.Instance.ElementSystem.LearnElement(0);
            UpdateValues();
        }

        private void LearnDarkness()
        {
            GM.Instance.ElementSystem.LearnElement(1);
            UpdateValues();
        }

        private void LearnIce()
        {
            GM.Instance.ElementSystem.LearnElement(2);
            UpdateValues();
        }

        private void LearnIron()
        {
            GM.Instance.ElementSystem.LearnElement(3);
            UpdateValues();
        }

        private void LearnStorm()
        {
            GM.Instance.ElementSystem.LearnElement(4);
            UpdateValues();
        }

        private void LearnNature()
        {
            GM.Instance.ElementSystem.LearnElement(5);
            UpdateValues();
        }

        private void LearnFire()
        {
            GM.Instance.ElementSystem.LearnElement(6);
            UpdateValues();
        }
    }
}
