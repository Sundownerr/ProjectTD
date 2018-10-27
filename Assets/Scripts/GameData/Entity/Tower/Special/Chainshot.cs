using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Entity.Tower
{
    public class Chainshot : Special
    {
        public int BounceCount, DecreaseDamagePerBounce;

        public override void InitSpecial(TowerBaseSystem ownerTower)
        {
            ownerTower.Bullet.GetComponent<BulletSystem>().ChainshotCount = BounceCount;
            ownerTower.Bullet.GetComponent<BulletSystem>().RemainingBounceCount = BounceCount;
        }
    }
}
