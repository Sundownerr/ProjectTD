using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
    public class AuraSystem : EffectSystem
    {
        protected Range range;
        protected GameObject rangePrefab;

        public AuraSystem(Effect effect, EntitySystem owner) : base(effect, owner)
        {     
            this.effect = effect;
            this.owner = owner;       
        }     

        public override void Apply()   
        {
            isSet = true;
            isEnded = true;   
            rangePrefab = Object.Instantiate(GM.I.RangePrefab, 
                                owner.Prefab.transform.position, 
                                Quaternion.identity, 
                                owner.Prefab.transform);
            range = rangePrefab.GetComponent<Range>();    
           
        }

        public override void Continue()
        {
            if (owner == null)
                End();
        }

        public override void End()
        {                   
            Object.Destroy(rangePrefab);
        }
    }
}