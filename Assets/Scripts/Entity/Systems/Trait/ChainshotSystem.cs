using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
    public class ChainshotSystem : TraitSystem
    {
        private new Chainshot trait;
        public ChainshotSystem(EntitySystem owner, Chainshot trait) : base(owner, trait)
        {
            this.trait = trait;
        }

        public override void Init()
        {
            if(owner is TowerSystem tower)
            {
                tower.Bullet.GetComponent<BulletSystem>().ChainshotCount = trait.BounceCount;
                tower.Bullet.GetComponent<BulletSystem>().RemainingBounceCount = trait.BounceCount;
            }
        }

    }
}