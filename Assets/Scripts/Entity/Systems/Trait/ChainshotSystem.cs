using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
    public class ChainshotSystem : IBulletTraitSystem
    {
        private Chainshot trait;

        public ChainshotSystem(Chainshot trait) 
        {
            this.trait = trait;          
        }

        public void IncreaseStatsPerLevel()
        {
            Debug.Log("incresa");
        }

        public void Apply(BulletSystem bullet)
        {                   
            var tower = trait.Owner as TowerSystem;
            var chainColliderList = new Collider[40];
            var creepLayer = 1 << 12;

            var hitTargetCount = Physics.OverlapSphereNonAlloc(
                bullet.Prefab.transform.position, 
                150, 
                chainColliderList, 
                creepLayer);    
        
            if (bullet.Target != null)         
            {   
                DamageSystem.DoDamage(bullet.Target, tower.Stats.Damage.Value, tower);

                if (bullet.RemainingBounceCount <= 0)                                 
                    tower.CombatSystem.SetTargetReached(bullet);           
                else
                {            
                    for (int i = 0; i < hitTargetCount; i++)                                                    
                        if (bullet.Target.Prefab == chainColliderList[i].gameObject)
                        {
                            bullet.Target = 

                                i - 1 >= 0 ? 
                                    GM.I.CreepList.Find(creep => 
                                        creep.Prefab == chainColliderList[i - 1].transform.gameObject) :

                                i + 1 < hitTargetCount ? 
                                    GM.I.CreepList.Find(creep => 
                                        creep.Prefab == chainColliderList[i + 1].transform.gameObject) :

                                bullet.Target;     
                            break; 
                        }                             
                    bullet.RemainingBounceCount--;       
                }               
            }                              
        }
    }
}