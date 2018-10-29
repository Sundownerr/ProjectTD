using Game.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Data.Entity.Tower
{
    [CreateAssetMenu(fileName = "Multishot", menuName = "Data/Tower/Special/Multishot")]

    public class Multishot : Special
    {
        public int Count;

        private void Awake()
        {
            SpecialName = "Multishot";
            SpecialDescription = $"Shoot {Count} additional targets";
        }

        public override void InitSpecial(TowerBaseSystem ownerTower)
        {
            ownerTower.Bullet.GetComponent<BulletSystem>().MultishotCount = Count;
        }
    }
}
