using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using Game.Systems.Effects;
using UnityEngine;

namespace Game.Data.Effects
{
	[CreateAssetMenu(fileName = "DoT", menuName = "Data/Effect/DoT")]
	
	public class DoT : Effect
	{
        public int DamagePerTick;
		public GameObject EffectPrefab;
	
		public override EffectSystem GetEffectSystem() => new DoTSystem(this, owner);
    }
}