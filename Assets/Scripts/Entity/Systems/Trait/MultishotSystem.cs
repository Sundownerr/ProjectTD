using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
	public class MultishotSystem : TraitSystem
    {
        private new Multishot trait;
        public MultishotSystem(EntitySystem owner, Multishot trait) : base(owner, trait)
        {
            this.trait = trait;
        }

        public override void Init()
        {
            if(owner is TowerSystem tower)
            {
                tower.Bullet.GetComponent<BulletSystem>().MultishotCount = trait.Count;
            }
        }

    }
}