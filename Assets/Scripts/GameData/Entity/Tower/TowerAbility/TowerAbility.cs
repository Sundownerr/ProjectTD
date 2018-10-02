using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Entity.Tower
{
    public class TowerAbilty : ScriptableObject
    {

        public string abilityName, abilityDescription;

        public float manaCost, cooldown, triggerChance;
    }
}