using System;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Data.Effect
{

    [Serializable]
    public class Effect : ScriptableObject
    {
        public string EffectName = "effectname", EffectDescription = "effectname";
        public float Duration = 0;
    }
}
