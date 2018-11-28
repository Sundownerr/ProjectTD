using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Systems;
using System;
using Game.Creep;
using Game.Tower;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Data/Ability")]

    [Serializable]
    public class Ability : Entity
    {     
        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect> EffectList;       
    }
}