using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
	public class EffectSystem
	{
		public bool IsSet { get => isSet; set => isSet = value; }
        public bool IsEnded { get => isEnded; set => isEnded = value; }
        public bool IsMaxStackCount { get => isMaxStackCount; set => isMaxStackCount = value; }
        public EntitySystem Target { get => target; set => target = value; }

        protected bool isSet, isEnded, isMaxStackCount;
        protected float effectTimer;
        protected EntitySystem target, owner;
		protected Effect effect; 
        protected List<int> id;
        protected AbilitySystem ownerAbilitySystem;


        public EffectSystem(Effect effect, EntitySystem owner)
        {		
            this.owner = owner;
            this.effect = effect;

            if (!effect.IsStackable)
                effect.MaxStackCount = 1;
        }

        public void SetId() 
        {   
            id = new List<int>();
            if (owner is Tower.TowerSystem tower)
            {               
                id.AddRange(tower.Stats.Id);  
                id.Add(ownerAbilitySystem.EffectSystemList.IndexOf(this));
            }
        }

        public void SetOwner(EntitySystem owner, AbilitySystem ownerAbilitySystem)
        {
            this.owner = owner;
            this.ownerAbilitySystem = ownerAbilitySystem;
            SetId();
        }     

        public virtual void Init()
        {
            if (!IsSet)
                Apply();
            
            Continue();
        }

        public virtual void Apply()
        {       
            if (!(this is AuraSystem))   
            {  
                if (effect.IsStackable)
                    if (Target.AppliedEffectSystem.CountOf(effect) >= effect.MaxStackCount)
                    {
                        IsMaxStackCount = true;                
                        return;
                    }           
                
                IsSet = true;
                IsEnded = false;
            }
        }

        public virtual void Continue()
        {       
            if (!(this is AuraSystem))
                if (!IsEnded)
                {
                    if (Target == null)
                        End();     
                        
                    effectTimer = effectTimer > effect.Duration ? -1 : effectTimer += Time.deltaTime;

                    if (effectTimer == -1)
                        End();
                }             
        }

        public virtual void End() 
        {     
            if (!(this is AuraSystem))   
            {
                if (!IsMaxStackCount)     
                    Target?.AppliedEffectSystem.Remove(effect);

                IsEnded = true;     
            }  
        } 
  
        public virtual void ApplyRestart()
        {
            if (!(this is AuraSystem))   
            {
                if (effect.IsStackable)
                    RestartState();
                else if (IsEnded)
                    RestartState();     
            }
        }

        public virtual void RestartState()
        {     
            if (!(this is AuraSystem))   
            {     
                End();
                IsMaxStackCount = false;
                IsEnded = false;
                IsSet = false;          
            }
        }    
		
        public virtual void SetTarget(EntitySystem newTarget) 
        {
            Target = Target ?? newTarget; 
        }                            
    }	
}
