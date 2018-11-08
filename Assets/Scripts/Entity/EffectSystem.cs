using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Data.Effects;
using UnityEngine;

namespace Game.Systems
{
	public class EffectSystem 
	{
		public List<Effect> AppliedEffectList { get => appliedEffectList; set => appliedEffectList = value; }

		private List<Effect> appliedEffectList = new List<Effect>();

        public void ApplyEffect(Effect effect)
		{
			appliedEffectList.Add(effect);
		}

		public void RemoveEffect(Effect effect)
		{
			for (int i = 0; i < appliedEffectList.Count; i++)			
				if(effect.CompareId(appliedEffectList[i].Id)) 
				{			
					appliedEffectList.RemoveAt(i);
					return;
				}
		}

		public int CountOf(Effect effect)
		{
			var count = 0;
			for (int i = 0; i < appliedEffectList.Count; i++)			
				if(effect.CompareId(appliedEffectList[i].Id)) 
				{			
					count++;
				}
			return count;
		}		
	}
}
