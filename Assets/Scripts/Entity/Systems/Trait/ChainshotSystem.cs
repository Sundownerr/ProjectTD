using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
    public class ChainshotSystem : IBulletTraitSystem
    {
        public EntitySystem Owner { get => owner; set => owner = value; }

        private EntitySystem owner;
        private Chainshot trait;

        public ChainshotSystem(Chainshot trait, EntitySystem owner) 
        {
            this.trait = trait;      
            Owner = owner;    
        }

        public void IncreaseStatsPerLevel()
        {
           //Debug.Log("incresa");
        }

        public void Apply(BulletSystem bullet)
        {                   
            var tower = Owner as TowerSystem;
            var colliders = new Collider[40];
            var creepLayer = 1 << 12;

            var hitTargetCount = Physics.OverlapSphereNonAlloc(
                bullet.Prefab.transform.position, 
                150, 
                colliders, 
                creepLayer);    
        
            if (bullet.Target != null)         
            {   
                DamageSystem.DoDamage(bullet.Target, tower.Stats.Damage.Value, tower);

                if (bullet.RemainingBounceCount <= 0)                                 
                    tower.CombatSystem.SetTargetReached(bullet);           
                else
                {            
                    for (int i = 0; i < hitTargetCount; i++)                                                    
                        if (bullet.Target.Prefab == colliders[i].gameObject)
                        {
                            bullet.Target = 

                                i - 1 >= 0 ? 
                                    GM.I.Creeps.Find(creep => 
                                        creep.Prefab == colliders[i - 1].transform.gameObject) :

                                i + 1 < hitTargetCount ? 
                                    GM.I.Creeps.Find(creep => 
                                        creep.Prefab == colliders[i + 1].transform.gameObject) :

                                bullet.Target;     
                            break; 
                        }                             
                    bullet.RemainingBounceCount--;       
                }               
            }                              
        }
    }
}