using System.Collections;
using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class AOEShotSystem : BulletTraitSystem
    {
        private new AOEShot trait;
        private Collider[] aoeColliderList;
        private int creepLayer;      

        public AOEShotSystem(AOEShot trait, EntitySystem owner) : base(trait, owner)
        {
            this.trait = trait;
            aoeColliderList = new Collider[40];
            creepLayer = 1 << 12;
        }

        public override void IncreaseStatsPerLevel()
        {
            Debug.Log("increase stats per level");
        }

        public override void Apply(BulletSystem bullet)
        {
            var tower = owner as TowerSystem;
            var hitTargetCount = Physics.OverlapSphereNonAlloc(
            bullet.Prefab.transform.position, 
            trait.Range, 
            aoeColliderList, 
            creepLayer);

            for (int i = 0; i < hitTargetCount; i++)
                DamageSystem.DoDamage(
                    GM.I.CreepList.Find(creep => creep.Prefab == aoeColliderList[i].transform.gameObject), 
                    tower.Stats.Damage.Value, 
                    tower);          
        }
    }
}