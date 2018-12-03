using UnityEngine;

namespace Game.Tower.Data.Stats.Specials
{
    [CreateAssetMenu(fileName = "Chainshot", menuName = "Data/Tower/Special/Chainshot")]

    public class Chainshot : Trait
    {
        public int BounceCount, DecreaseDamagePerBounce;

        private void Awake()
        {
            Name = "Chainshot";
            Description = $"Bounce between {BounceCount} targets";
        }

        public override void InitTrait(TowerSystem ownerTower)
        {
            ownerTower.Bullet.GetComponent<BulletSystem>().ChainshotCount = BounceCount;
            ownerTower.Bullet.GetComponent<BulletSystem>().RemainingBounceCount = BounceCount;
        }
    }
}
