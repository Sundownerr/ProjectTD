using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Multishot", menuName = "Data/Tower/Special/Multishot")]

    public class Multishot : Trait
    {
        public int Count;

        private void Awake()
        {
            Name = "Multishot";
            Description = $"Shoot {Count} additional targets";
        }

        public override void InitTrait(TowerSystem ownerTower)
        {
            ownerTower.Bullet.GetComponent<BulletSystem>().MultishotCount = Count;
        }
    }
}
