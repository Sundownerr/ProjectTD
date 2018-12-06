using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class AOEShotSystem : IBulletTraitSystem
    {
        private  AOEShot trait;   

        public AOEShotSystem(AOEShot trait)
        {
             this.trait = trait;                       
        }

        public void IncreaseStatsPerLevel()
        {
            Debug.Log("increase stats per level");
        }

        public void Apply(BulletSystem bullet)
        {
            var creepLayer      = 1 << 12;
            var colliderList    = new Collider[40];
            var tower           = trait.Owner as TowerSystem;

            var hitTargetCount = Physics.OverlapSphereNonAlloc(
            bullet.Prefab.transform.position, 
            trait.Range, 
            colliderList, 
            creepLayer);

            for (int i = 0; i < hitTargetCount; i++)
                DamageSystem.DoDamage(
                    GM.I.CreepList.Find(creep => creep.Prefab == colliderList[i].transform.gameObject), 
                    tower.Stats.Damage.Value, 
                    tower);          
        }
    }
}