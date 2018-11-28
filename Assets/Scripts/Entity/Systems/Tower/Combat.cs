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

        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private List<float> bulletRemoveTimerList;
        private TowerSystem tower;
        private ObjectPool bulletPool;
        private float timer;
        private BulletSystem defaultBullet;

        public Combat(TowerSystem ownerTower) => tower = ownerTower;

        private void OnDestroy() => bulletPool.DestroyPool();

        public void Set()
        {
            timer = tower.Stats.AttackSpeed;
            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();
            bulletRemoveTimerList = new List<float>();
            defaultBullet = tower.Bullet.GetComponent<BulletSystem>();

            bulletPool = new ObjectPool()
            {
                PoolObject = tower.Bullet,
                Parent = tower.Prefab.transform
            };

            bulletPool.Initialize();
        }

        private void CreateBullet(CreepSystem target)
        {
            bulletList.Add(bulletPool.GetObject());
            bulletDataList.Add(bulletList[bulletList.Count - 1].GetComponent<BulletSystem>());
            SetBulletData(bulletDataList[bulletDataList.Count - 1]);

            bulletList[bulletList.Count - 1].SetActive(true);

            void SetBulletData(BulletSystem bullet)
            {
                if(target != null)                
                    bullet.Target = target;                  
                
                bullet.ChainshotCount = defaultBullet.ChainshotCount;
                bullet.AOEShotRange = defaultBullet.AOEShotRange;
                bullet.transform.position = tower.ShootPoint.position;
                bullet.transform.rotation = tower.MovingPart.rotation;
            }
        }

        private void SetTargetReached(BulletSystem bullet)
        {
            if (!bullet.IsTargetReached)
            {
                bullet.IsTargetReached = true;
                bullet.Show(false);
                bulletRemoveTimerList.Add(bulletDataList[bulletList.Count - 1].Lifetime);
            }              
        }

        public void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
                if (bulletList[i].activeSelf)                                 
                    if (bulletDataList[i].IsTargetReached)
                        SetTargetReached(bulletDataList[i]);
                    else
                    {
                        var offset = new Vector3(0, 40, 0);
                        var distance = QoL.CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.Prefab.transform.position + offset);

                        if (distance < 30)
                            HitTarget(bulletDataList[i]);
                        else
                        {
                            var randVec = new Vector3(
                                UnityEngine.Random.Range(-10, 10),
                                UnityEngine.Random.Range(-10, 10),
                                UnityEngine.Random.Range(-10, 10));

                            bulletList[i].transform.LookAt(bulletDataList[i].Target.Prefab.transform.position + offset);
                            bulletList[i].transform.Translate(Vector3.forward * bulletDataList[i].Speed + randVec, Space.Self);
                        }
                    }                                     
        }

        public bool CheckAllBulletInactive()
        {
            for (int i = 0; i < bulletList.Count; i++)
                if (bulletList[i].activeSelf)
                    return false;

            return true;
        }

        private void RemoveBullet(BulletSystem bullet)
        {      
            bullet.gameObject.SetActive(false);
            bulletDataList.Remove(bullet);
            bulletList.Remove(bullet.gameObject);
        }

        private void HitTarget(BulletSystem bullet)
        {
            if (bullet.Target == null || bullet.Target.Prefab == null)
            {
                SetTargetReached(bullet);
                return;
            }

            var isChainShot =
                bullet.ChainshotCount > 0 &&
                bullet.RemainingBounceCount > 0;        

            if(bullet.AOEShotRange > 0)
                tower.SpecialSystem.DamageInAOE(bullet);
            else
                ApplyDamage();

            if(isChainShot)
                tower.SpecialSystem.SetChainTarget(bullet);
            else
                SetTargetReached(bullet);         

            void ApplyDamage()
            {
                if (bullet.Target != null)
                    DamageSystem.DoDamage(bullet.Target, tower.Stats.Damage.Value, tower);
            }
        }

        public void UpdateSystem()
        {
            timer = timer > tower.Stats.AttackSpeed ? 0 : timer + Time.deltaTime * 0.5f;
            MoveBullet();

            if (timer > tower.Stats.AttackSpeed)
                ShotBullet();

            void ShotBullet()
            {
                var shotCount = tower.SpecialSystem.CalculateShotCount();

                for (int i = 0; i < shotCount; i++)
                    CreateBullet(tower.CreepInRangeList[i]);
            }

            for (int i = 0; i < bulletRemoveTimerList.Count; i++)
            {
                if(bulletRemoveTimerList[i] > 0)
                    bulletRemoveTimerList[i] -= Time.deltaTime;
                else
                {
                    RemoveBullet(bulletDataList[0]);
                    bulletRemoveTimerList.RemoveAt(i);
                }
            }
        }   
    }
}