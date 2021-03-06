﻿using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Chainshot", menuName = "Data/Tower/Trait/Chainshot")]

    public class Chainshot : Trait, ITrait
    {
        public int BounceCount, DecreaseDamagePerBounce;

        private void Awake()
        {
            Name = "Chainshot";
            Description = $"Bounce between {BounceCount} targets";
        }

        public override ITraitSystem GetTraitSystem(EntitySystem owner) => new ChainshotSystem(this, owner);
    }
}
