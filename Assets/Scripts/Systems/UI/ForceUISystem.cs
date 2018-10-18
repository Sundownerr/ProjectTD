using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.System
{
    public class ForceUISystem : ExtendedMonoBehaviour
    {
        public Button Astral, Darkness, Ice, Iron, Storm, Nature, Fire;

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

            ShowElementButtons(false);
        }

        public void ShowElementButtons(bool show)
        {
            Astral.gameObject.SetActive(show);
            Darkness.gameObject.SetActive(show);
            Ice.gameObject.SetActive(show);
            Iron.gameObject.SetActive(show);
            Storm.gameObject.SetActive(show);
            Nature.gameObject.SetActive(show);
            Fire.gameObject.SetActive(show);
        }

        private void LearnAstral()
        {
            GM.Instance.ForceSystem.LearnAstral();
        }

        private void LearnDarkness()
        {
            GM.Instance.ForceSystem.LearnDarkness();
        }

        private void LearnIce()
        {
            GM.Instance.ForceSystem.LearnIce();
        }

        private void LearnIron()
        {
            GM.Instance.ForceSystem.LearnIron();
        }

        private void LearnStorm()
        {
            GM.Instance.ForceSystem.LearnStorm();
        }

        private void LearnNature()
        {
            GM.Instance.ForceSystem.LearnNature();
        }

        private void LearnFire()
        {
            GM.Instance.ForceSystem.LearnFire();
        }
    }
}
