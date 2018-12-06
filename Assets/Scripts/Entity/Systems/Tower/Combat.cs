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
        public int ShotCount { get => shotCount; set => shotCount = value; }
        public bool isHaveChainTargets;
        public event EventHandler<BulletSystem> BulletHit = delegate{};
        public event EventHandler PrepareToShoot = delegate{};
        public event EventHandler<BulletSystem> Shooting = delegate{};

        private List<BulletSystem> bulletList;
        private List<GameObject> bulletGOList;
        private List<float> removeTimerList;
        private TowerSystem tower;
        private ObjectPool bulletPool;
        private float attackDelay;
        private int shotCount;

        public Combat(TowerSystem ownerTower) => tower = ownerTower;

        private void OnDestroy() => bulletPool.DestroyPool();

        public void Set()
        {
            attackDelay     = QoL.GetPercentOfValue(tower.Stats.AttackSpeedModifier, tower.Stats.AttackSpeed);         
            bulletGOList    = new List<GameObject>();
            bulletList      = new List<BulletSystem>();
            removeTimerList = new List<float>();       

            bulletPool = new ObjectPool()
            {
                PoolObject = tower.Bullet,
                Parent = tower.Prefab.transform
            };

            bulletPool.Initialize();
        }

        public void UpdateSystem()
        {
            var modifiedAttackSpeed = QoL.GetPercentOfValue(tower.Stats.AttackSpeedModifier, tower.Stats.AttackSpeed);
            
            var attackCooldown = tower.Stats.AttackSpeedModifier < 100 ? 
                    tower.Stats.AttackSpeed + (tower.Stats.AttackSpeed - modifiedAttackSpeed) : 
                    tower.Stats.AttackSpeed - (modifiedAttackSpeed - tower.Stats.AttackSpeed);
                         
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

            #region Helper functions

            void ShotBullet()
            {              
                PrepareToShoot?.Invoke(this, new EventArgs());
 
                for (int i = 0; i < shotCount; i++)
                    CreateBullet(tower.CreepInRangeList[i]);

                void CreateBullet(EntitySystem target)
                {
                    var bulletGO = bulletPool.GetObject();

                    bulletGOList.Add(bulletGO);
                    bulletList.Add(new BulletSystem(bulletGO));

                    SetBulletData(bulletList[bulletList.Count - 1]);

                    Shooting?.Invoke(this, bulletList[bulletList.Count - 1]);
                    bulletGOList[bulletGOList.Count - 1].SetActive(true);

                    void SetBulletData(BulletSystem bullet)
                    {                                                        
                        bulletGO.transform.position = tower.ShootPoint.position;
                        bulletGO.transform.rotation = tower.MovingPart.rotation;                       
                        bullet.Show(true);
                        bullet.IsTargetReached = false;

                        if (target != null)                
                            bullet.Target = target; 
                        else
                            RemoveBullet(bullet);  
                    }
                }
            }

            void RemoveBullet(BulletSystem bullet)
            {                  
                bullet.Show(false);         
                bullet.Prefab.SetActive(false);
                bulletList.Remove(bullet);
                bulletGOList.Remove(bullet.Prefab);
            }

            #endregion
        }   
   
        public void SetTargetReached(BulletSystem bullet)
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
                if (bullet.Prefab.activeSelf) 
                    if (!bullet.IsTargetReached)          
                        if (bullet.Target == null || bullet.Target.Prefab == null)
                            SetTargetReached(bullet);
                        else
                        {   
                            var bulletGO = bullet.Prefab;                             
                            var offset = new Vector3(0, 40, 0);
                            var distance = QoL.CalcDistance(bulletGO.transform.position, bullet.Target.Prefab.transform.position + offset);

                            if (distance < 30)
                                HitTarget(bullet);
                            else
                            {
                                var randVec = new Vector3(
                                    UnityEngine.Random.Range(-10, 10),
                                    UnityEngine.Random.Range(-10, 10),
                                    UnityEngine.Random.Range(-10, 10));

                                bulletGO.transform.LookAt(bullet.Target.Prefab.transform.position + offset);
                                bulletGO.transform.Translate(Vector3.forward * bullet.Speed + randVec, Space.Self);
                            }                         
                        }          
            }                        
        }

        private void HitTarget(BulletSystem bullet) => BulletHit?.Invoke(this, bullet);                  
    }
}