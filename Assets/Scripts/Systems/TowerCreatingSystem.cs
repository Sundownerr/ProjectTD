using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class TowerCreatingSystem : ExtendedMonoBehaviour
    {
        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.TowerCreatingSystem = this;
        }       
    }
}