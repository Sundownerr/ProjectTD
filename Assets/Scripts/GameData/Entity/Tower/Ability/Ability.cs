using Game.Data.Effect;
using Game.System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Base Ability")]
    
    public class Ability : ScriptableObject
    {
        public int TowerID;

        public string AbilityName, AbilityDescription;

        public float ManaCost, Cooldown, TriggerChance;
        
        public List<Effect.Effect> EffectList;
    }
}