using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Data.Effects
{
	public class Aura : Effect 
	{
		public float Range;	

		public override EffectSystem GetEffectSystem() => new AuraSystem(this, owner);

		
	}
}
