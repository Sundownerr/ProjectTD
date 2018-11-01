using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "Chainshot", menuName = "Data/Tower/Special/Chainshot")]

    public class Chainshot : Special
    {
        public int BounceCount, DecreaseDamagePerBounce;

        private void Awake()
        {
            SpecialName = "Chainshot";
            SpecialDescription = $"Bounce between {BounceCount} targets";
        }

        public override void InitSpecial(TowerSystem ownerTower)
        {
            ownerTower.Bullet.GetComponent<BulletSystem>().ChainshotCount = BounceCount;
            ownerTower.Bullet.GetComponent<BulletSystem>().RemainingBounceCount = BounceCount;
        }
    }
}
