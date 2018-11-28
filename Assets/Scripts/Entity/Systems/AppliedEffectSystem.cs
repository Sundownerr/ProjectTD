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
		private List<Effect> appliedEffectList = new List<Effect>();

        public void Add(Effect effect) => appliedEffectList.Add(effect);	

		public void Remove(Effect effect)
		{
			for (int i = 0; i < appliedEffectList.Count; i++)			
				if (effect.CompareId(appliedEffectList[i].Id)) 
				{			
					appliedEffectList.RemoveAt(i);
					return;
				}
		}

		public int CountOf(Effect effect)
		{
			var count = 0;
			for (int i = 0; i < appliedEffectList.Count; i++)			
				if (effect.CompareId(appliedEffectList[i].Id)) 						
					count++;			
			return count;
		}		
	}
}
