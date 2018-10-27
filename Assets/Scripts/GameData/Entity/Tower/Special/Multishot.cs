using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Data.Entity.Tower
{
    public class Multishot : Special
    {
        public int Count;

        public override void InitSpecial(TowerBaseSystem ownerTower)
        {
            ownerTower.Bullet.GetComponent<BulletSystem>().MultishotCount = Count;
        }
    }
}
