using System;
using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    [Serializable]
    public class Combat
    {
        public bool isHaveChainTargets;

        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
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
            defaultBullet = tower.Bullet.GetComponent<BulletSystem>();

            bulletPool = new ObjectPool()
            {
                PoolObject = tower.Bullet,
                Parent = tower.transform
            };

            bulletPool.Initialize();
        }

        private void CreateBullet(EntitySystem target)
        {
            bulletList.Add(bulletPool.GetObject());
            bulletDataList.Add(bulletList[bulletList.Count - 1].GetComponent<BulletSystem>());
            SetBulletData(bulletDataList[bulletDataList.Count - 1]);

            bulletList[bulletList.Count - 1].SetActive(true);

            void SetBulletData(BulletSystem bullet)
            {
                if(target != null)
                {
                    bullet.Target = target.gameObject;
                    bullet.ChainshotCount = defaultBullet.ChainshotCount;
                    bullet.AOEShotRange = defaultBullet.AOEShotRange;
                    bullet.transform.position = tower.ShootPointTransform.position;
                    bullet.transform.rotation = tower.MovingPartTransform.rotation;
                }
            }
        }

        private void SetTargetReached(BulletSystem bullet)
        {
            if (!bullet.IsTargetReached)
                bullet.StartCoroutine(RemoveBullet(bullet));
        }

        public void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
                if (bulletList[i].activeSelf)
                    if (bulletDataList[i].Target == null)
                        HitTarget(bulletDataList[i]);
                    else
                    {                 
                        if (bulletDataList[i].IsTargetReached)
                            SetTargetReached(bulletDataList[i]);
                        else
                        {
                            var offset = new Vector3(0, 40, 0);
                            var distance = QoL.CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.transform.position + offset);

                            if (distance < 30)
                                HitTarget(bulletDataList[i]);
                            else
                            {
                                var randVec = new Vector3(
                                    UnityEngine.Random.Range(-10, 10),
                                    UnityEngine.Random.Range(-10, 10),
                                    UnityEngine.Random.Range(-10, 10));

                                bulletList[i].transform.LookAt(bulletDataList[i].Target.transform.position + offset);
                                bulletList[i].transform.Translate(Vector3.forward * bulletDataList[i].Speed + randVec, Space.Self);
                            }
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

        private IEnumerator RemoveBullet(BulletSystem bullet)
        {
            bullet.IsTargetReached = true;
            bullet.Show(false);

            yield return new WaitForSeconds(bullet.Lifetime);

            bullet.gameObject.SetActive(false);
            bulletDataList.Remove(bullet);
            bulletList.Remove(bullet.gameObject);
        }
 
        private delegate void HitAction(BulletSystem bullet);
        HitAction hitAction;
        private void HitTarget(BulletSystem bullet)
        {
            var isChainShot =
                bullet.ChainshotCount > 0 &&
                bullet.RemainingBounceCount > 0;

        
            hitAction += bullet.AOEShotRange > 0 ? tower.SpecialSystem.DamageInAOE : (HitAction)ApplyDamage;
            hitAction += isChainShot ? tower.SpecialSystem.SetChainTarget : (HitAction)SetTargetReached;

            hitAction?.Invoke(bullet);
            hitAction = null;

            if (bullet.Target == null)
                SetTargetReached(bullet);

            void ApplyDamage(BulletSystem bulletе)
            {
                if (bulletе.Target != null)
                    DamageSystem.DoDamage(bulletе.Target.GetComponent<Creep.CreepSystem>(), tower.Stats.Damage.Value, tower);
            }
        }

        public void Update()
        {
            timer = timer > tower.Stats.AttackSpeed ? 0 : timer += Time.deltaTime;
            MoveBullet();

            if (timer > tower.Stats.AttackSpeed)
                ShotBullet();

            void ShotBullet()
            {
                var shotCount = tower.SpecialSystem.CalculateShotCount();

                for (int i = 0; i < shotCount; i++)
                    CreateBullet(tower.CreepInRangeList[i]);
            }
        }   
    }
}