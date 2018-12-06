using System;
using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Data;
using Game.Tower;
using UnityEngine;
namespace Game.Systems
{
	public class AbilitySystem 
	{
		public bool IsNeedStack { get => isNeedStack; set => isNeedStack = value; }
        public EntitySystem Target { get => target; set => target = value; }
        public bool IsStacked { get => isStacked; set => isStacked = value; }
        public List<EffectSystem> EffectSystems { get => effectSystems; set => effectSystems = value; }

        private Ability ability;
		private bool isStacked, isNeedStack;
        private EntitySystem target;      
        private int effectCount;
        private float cooldownTimer, nextEffectTimer;
		private List<EffectSystem> effectSystems;
        private EntitySystem owner;
        private List<int> id;

		public AbilitySystem(Ability ability, EntitySystem owner)
		{
			this.ability = ability;       

            effectSystems = new List<EffectSystem>();
			for (int i = 0; i < ability.Effects.Count; i++)		           
				EffectSystems.Add(ability.Effects[i].GetEffectSystem());           
            SetOwner(owner);		
		}

        public void SetId() 
        {
            id = new List<int>();          
            if (owner is TowerSystem tower)
            {
                id.AddRange(tower.Stats.Id);                             
                id.Add(tower.AbilitySystems.IndexOf(this));                
            }
        }         

        private void SetOwner(EntitySystem owner)
        {                 
            this.owner = owner;
            SetId();

            for (int i = 0; i < EffectSystems.Count; i++)           
                EffectSystems[i].SetOwner(owner, this);
            
            ability.Effects[ability.Effects.Count - 1].NextInterval = 0.01f;               
        }

		public void Init() 
        {
            if (!IsStacked)
                if (cooldownTimer < ability.Cooldown)                    
                    cooldownTimer += Time.deltaTime;                                
                else
                {        
                    cooldownTimer = 0;             
                    IsNeedStack = CheckNeedStack();
                    CooldownReset();   
                }         

            nextEffectTimer = nextEffectTimer > ability.Effects[effectCount].NextInterval ? 0 : nextEffectTimer + Time.deltaTime;

            for (int i = 0; i <= effectCount; i++)
                EffectSystems[i].Init();  

            if (!(effectCount >= ability.Effects.Count - 1))
                if (nextEffectTimer > ability.Effects[effectCount].NextInterval)                  
                    effectCount++;   

            #region  Helper functions
            bool CheckNeedStack()
            {
                for (int i = 0; i < EffectSystems.Count; i++)
                    if (ability.Effects[i].IsStackable)
                    {
                        if (!EffectSystems[i].IsEnded)
                            return true;
                    }
                    else
                        if (EffectSystems[i].Target != target)
                            return true;
                return false;         
            }  
            #endregion
        }

        public void SetTarget(EntitySystem target)
        {
            Target = target;
          
            for (int i = 0; i < EffectSystems.Count; i++)
                EffectSystems[i].SetTarget(target);                     
        } 

        public void StackReset(EntitySystem owner)
        {
            IsStacked = true;                                  
            SetOwner(owner);  
        }

        public void CooldownReset()
        {
            effectCount = 0;        
            nextEffectTimer = 0;

            for (int i = 0; i < EffectSystems.Count; i++)
                EffectSystems[i].ApplyRestart();         
        }

        public bool CheckAllEffectsEnded()
        {
            for (int i = 0; i < EffectSystems.Count; i++)
                if (!EffectSystems[i].IsEnded)
                    return false;        
            return true;
        }
	}
}
