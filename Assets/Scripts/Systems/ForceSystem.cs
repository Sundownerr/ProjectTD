using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class ForceSystem : ExtendedMonoBehaviour
    {
        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.ForceSystem = this;
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
