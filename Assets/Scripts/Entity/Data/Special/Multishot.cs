using UnityEngine;

namespace Game.Tower.Data.Stats.Specials
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
