using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Systems
{
    public class AuraSystem : EffectSystem
    {
        public Range Range { get => range; set => range = value; }

        private Range range;
        private GameObject rangePrefab;

        public AuraSystem(Effect effect, EntitySystem owner) : base(effect, owner)
        {
            this.effect = effect;
            rangePrefab = Object.Instantiate(GM.I.RangePrefab, 
                                owner.Prefab.transform.position, 
                                Quaternion.identity, 
                                owner.Prefab.transform);
            range = rangePrefab.GetComponent<Range>();
            range.CollideType = CollideWith.CreepsAndTowers;
        }     

        public override void Apply()   
        {
            isEnded = true;
            isSet = true;
        }

        public override void Continue()
        {

        }

        public override void End()
        {         
            Object.Destroy(rangePrefab);
        }
    }
}