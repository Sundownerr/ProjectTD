using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "SlowAura", menuName = "Data/Effect/Slow Aura")]
	public class SlowAura : Effect
	{
		public float Size;
		public float SlowPercent;
        public GameObject EffectPrefab;
		public override EffectSystem GetEffectSystem() => new SlowAuraSystem(this, owner);
	}
}
