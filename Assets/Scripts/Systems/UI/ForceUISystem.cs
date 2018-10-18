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

        }

        private void LearnDarkness()
        {

        }

        private void LearnIce()
        {

        }

        private void LearnIron()
        {

        }

        private void LearnStorm()
        {

        }

        private void LearnNature()
        {

        }

        private void LearnFire()
        {

        }
    }
}
