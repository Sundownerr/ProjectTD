using System;
using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    [Serializable]
    public class Combat : ITowerSystem
    {
        public bool isHaveChainTargets;

        private List<BulletSystem> bulletList;
        private List<GameObject> bulletGOList;
        private List<float> removeTimerList;
        private TowerSystem tower;
        private ObjectPool bulletPool;
        private float attackDelay, attackCooldown;
        private BulletSystem defaultBullet;

        public Combat(TowerSystem ownerTower) => tower = ownerTower;

        private void OnDestroy() => bulletPool.DestroyPool();

        public void Set()
        {
            attackDelay     = QoL.GetPercentOfValue(tower.Stats.AttackSpeedModifier, tower.Stats.AttackSpeed);         
            bulletGOList    = new List<GameObject>();
            bulletList      = new List<BulletSystem>();
            removeTimerList = new List<float>();
            defaultBullet   = tower.Bullet.GetComponent<BulletSystem>();

            bulletPool = new ObjectPool()
            {
                PoolObject = tower.Bullet,
                Parent = tower.Prefab.transform
            };

            bulletPool.Initialize();
        }

        public void UpdateSystem()
        {
            var attackSpeedMod = tower.Stats.AttackSpeedModifier;
            var percent = QoL.GetPercentOfValue(attackSpeedMod, tower.Stats.AttackSpeed);

            var attackCooldown = 
                attackSpeedMod < 100 ? 
                    tower.Stats.AttackSpeed + (tower.Stats.AttackSpeed - percent) : 
                    tower.Stats.AttackSpeed - (percent - tower.Stats.AttackSpeed);
                         
            attackDelay = attackDelay > attackCooldown ? 0 : attackDelay + Time.deltaTime * 0.5f;
            MoveBullet();

            if (attackDelay > attackCooldown)
                ShotBullet();      

            for (int i = 0; i < removeTimerList.Count; i++)          
                if (removeTimerList[i] > 0)
                    removeTimerList[i] -= Time.deltaTime;
                else
                {
                    RemoveBullet(bulletList[0]);
                    removeTimerList.RemoveAt(i);
                }
            
            void ShotBullet()
            {
                var shotCount = tower.SpecialSystem.CalculateShotCount();

                for (int i = 0; i < shotCount; i++)
                    CreateBullet(tower.CreepInRangeList[i]);

                void CreateBullet(EntitySystem target)
                {
                    bulletGOList.Add(bulletPool.GetObject());
                    bulletList.Add(bulletGOList[bulletGOList.Count - 1].GetComponent<BulletSystem>());
                    SetBulletData(bulletList[bulletList.Count - 1]);

                    bulletGOList[bulletGOList.Count - 1].SetActive(true);

                    void SetBulletData(BulletSystem bullet)
                    {                                                        
                        bullet.ChainshotCount = defaultBullet.ChainshotCount;
                        bullet.AOEShotRange = defaultBullet.AOEShotRange;
                        bullet.transform.position = tower.ShootPoint.position;
                        bullet.transform.rotation = tower.MovingPart.rotation;

                        if (target != null)                
                            bullet.Target = target; 
                        else
                            RemoveBullet(bullet);  
                    }
                }
            }

            void RemoveBullet(BulletSystem bullet)
            {      
                bullet.gameObject.SetActive(false);
                bulletList.Remove(bullet);
                bulletGOList.Remove(bullet.gameObject);
            }
        }   
   
        private void SetTargetReached(BulletSystem bullet)
        {
            if (!bullet.IsTargetReached)
            {
                bullet.IsTargetReached = true;
                bullet.Show(false);
                removeTimerList.Add(bulletList[bulletGOList.Count - 1].Lifetime);
            }              
        }

        public void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                var bullet = bulletList[i];
                if (bullet.gameObject.activeSelf) 
                    if (!bullet.IsTargetReached)          
                        if (bullet.Target == null || bullet.Target.Prefab == null)
                            SetTargetReached(bullet);
                        else
                        {                                
                            var offset = new Vector3(0, 40, 0);
                            var distance = QoL.CalcDistance(bullet.transform.position, bullet.Target.Prefab.transform.position + offset);

                            if (distance < 30)
                                HitTarget(bullet);
                            else
                            {
                                var randVec = new Vector3(
                                    UnityEngine.Random.Range(-10, 10),
                                    UnityEngine.Random.Range(-10, 10),
                                    UnityEngine.Random.Range(-10, 10));

                                bullet.transform.LookAt(bullet.Target.Prefab.transform.position + offset);
                                bullet.transform.Translate(Vector3.forward * bullet.Speed + randVec, Space.Self);
                            }                         
                        }          
            }                        
        }

        private void HitTarget(BulletSystem bullet)
        {
            var isChainShot =
                bullet.ChainshotCount > 0 &&
                bullet.RemainingBounceCount > 0;        

            if (bullet.AOEShotRange > 0)
                tower.SpecialSystem.DamageInAOE(bullet);
            else
                ApplyDamage();

            if (isChainShot)
                tower.SpecialSystem.SetChainTarget(bullet);
            else
                SetTargetReached(bullet);         

            void ApplyDamage()
            {
                if (bullet.Target != null)
                    DamageSystem.DoDamage(bullet.Target, tower.Stats.Damage.Value, tower);
            }
        }     
    }
}