using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;


namespace Game.Data
{
    [CreateAssetMenu(fileName = "AOE Shot", menuName = "Data/Tower/Trait/Multishot")]

    public class Armored : Trait, ITrait
    {
        public int AdditionalArmor;

        private void Awake()
        {
            Name = "Armored";
            Description = $"Add {AdditionalArmor} armor";
        }

        public override ITraitSystem GetTraitSystem(EntitySystem owner) => new ArmoredSystem(this, owner);
    }
}


