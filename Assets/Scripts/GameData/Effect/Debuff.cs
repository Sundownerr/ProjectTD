using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Effect
{
    public class Debuff : ScriptableObject
    {
        public string debuffName, debuffDescription;

        public float duration;
    }
}