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
        public EntitySystem Owner { get => owner; set => owner = value; }

        private EntitySystem owner;
        private AOEShot trait;   

        public AOEShotSystem(AOEShot trait, EntitySystem owner)
        {
             this.trait = trait;     
             Owner = owner;                   
        }

        public void IncreaseStatsPerLevel()
        {
            Debug.Log("increase stats per level");
        }

        public void Apply(BulletSystem bullet)
        {
            var creepLayer      = 1 << 12;
            var colliders       = new Collider[40];
            var tower           = Owner as TowerSystem;

            var hitTargetCount = Physics.OverlapSphereNonAlloc(
            bullet.Prefab.transform.position, 
            trait.Range, 
            colliders, 
            creepLayer);

            for (int i = 0; i < hitTargetCount; i++)
                DamageSystem.DoDamage(
                    GM.I.Creeps.Find(creep => creep.Prefab == colliders[i].transform.gameObject), 
                    tower.Stats.Damage.Value, 
                    tower);          
        }
    }
}