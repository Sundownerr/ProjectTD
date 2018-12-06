using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Data.Effects;
using UnityEngine;

namespace Game.Systems
{
	public class AppliedEffectSystem 
	{		
		private List<Effect> appliedEffects = new List<Effect>();

        public void Add(Effect effect) => appliedEffects.Add(effect);	

		public void Remove(Effect effect)
		{
			for (int i = 0; i < appliedEffects.Count; i++)			
				if (effect.CompareId(appliedEffects[i].Id)) 
				{			
					appliedEffects.RemoveAt(i);
					return;
				}
		}

		public int CountOf(Effect effect)
		{
			var count = 0;
			for (int i = 0; i < appliedEffects.Count; i++)			
				if (effect.CompareId(appliedEffects[i].Id)) 						
					count++;			
			return count;
		}		
	}
}
