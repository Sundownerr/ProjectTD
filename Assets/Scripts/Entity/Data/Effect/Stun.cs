using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using Game.Systems.Effects;
using UnityEngine;

namespace Game.Data.Effects
{
	[CreateAssetMenu(fileName = "Stun", menuName = "Data/Effect/Stun")]
	
	public class Stun : Effect
	{
		public GameObject EffectPrefab;	

		public override EffectSystem GetEffectSystem() => new StunSystem(this, owner);
	}
}