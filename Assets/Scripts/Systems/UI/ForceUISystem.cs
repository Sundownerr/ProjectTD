using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.System
{
    public class ForceUISystem : ExtendedMonoBehaviour
    {
        public Button Astral, Darkness, Ice, Iron, Storm, Nature, Fire;
        public TextMeshProUGUI AstralLevel, DarknessLevel, IceLevel, IronLevel, StormLevel, NatureLevel, FireLevel;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            Astral.onClick.AddListener(LearnAstral);
            Darkness.onClick.AddListener(LearnDarkness);
            Ice.onClick.AddListener(LearnIce);
            Iron.onClick.AddListener(LearnIron);
            Storm.onClick.AddListener(LearnStorm);
            Nature.onClick.AddListener(LearnNature);
            Fire.onClick.AddListener(LearnFire);

            GM.Instance.ForceUISystem = this;

            gameObject.SetActive(false);

            UpdateValues();
        }

        public void UpdateValues()
        {
            AstralLevel.text = GM.Instance.PlayerData.AstralLevel.ToString();
            DarknessLevel.text = GM.Instance.PlayerData.DarknessLevel.ToString();
            IceLevel.text = GM.Instance.PlayerData.IceLevel.ToString();
            IronLevel.text = GM.Instance.PlayerData.IronLevel.ToString();
            StormLevel.text = GM.Instance.PlayerData.StormLevel.ToString();
            NatureLevel.text = GM.Instance.PlayerData.NatureLevel.ToString();
            FireLevel.text = GM.Instance.PlayerData.FireLevel.ToString();
        }

        private void LearnAstral()
        {
            GM.Instance.ForceSystem.LearnAstral();
            UpdateValues();
        }

        private void LearnDarkness()
        {
            GM.Instance.ForceSystem.LearnDarkness();
            UpdateValues();
        }

        private void LearnIce()
        {
            GM.Instance.ForceSystem.LearnIce();
            UpdateValues();
        }

        private void LearnIron()
        {
            GM.Instance.ForceSystem.LearnIron();
            UpdateValues();
        }

        private void LearnStorm()
        {
            GM.Instance.ForceSystem.LearnStorm();
            UpdateValues();
        }

        private void LearnNature()
        {
            GM.Instance.ForceSystem.LearnNature();
            UpdateValues();
        }

        private void LearnFire()
        {
            GM.Instance.ForceSystem.LearnFire();
            UpdateValues();
        }
    }
}
