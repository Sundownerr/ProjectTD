using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;


namespace Game.Tower
{   
    [Serializable]
    public class TowerCombatSystem 
    {
        public StateMachine State;
        
        public bool isHaveChainTargets;

        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private TowerBaseSystem ownerTower;
        private ObjectPool bulletPool;
        private float timer;

        public TowerCombatSystem(TowerBaseSystem ownerTower)
        {          
            this.ownerTower = ownerTower;
        }

        private void OnDestroy()
        {
            bulletPool.DestroyPool();
        }

        public void Set()
        {
            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool();
            bulletPool.poolObject = ownerTower.Bullet;
            bulletPool.parent = ownerTower.transform;
            bulletPool.Initialize();

            State = new StateMachine();
            State.ChangeState(new ShootState(this));
            timer = ownerTower.StatsSystem.Stats.AttackSpeed;
        }

        private void CreateBullet(GameObject target)
        {
            bulletList.Add(bulletPool.GetObject());
            bulletDataList.Add(bulletList[bulletList.Count - 1].GetComponent<BulletSystem>());

            bulletDataList[bulletDataList.Count - 1].Target = target;
            bulletList[bulletList.Count - 1].transform.position = ownerTower.ShootPointTransform.position;
            bulletList[bulletList.Count - 1].transform.rotation = ownerTower.MovingPartTransform.rotation;

            bulletList[bulletList.Count - 1].SetActive(true);
        }

        private void SetTargetReached(BulletSystem bullet)
        {
            if (!bullet.IsTargetReached)
            {                    
                bullet.StartCoroutine(RemoveBullet(bullet));
            }
        }

        public void MoveBullet()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].activeSelf)
                {
                    if (bulletDataList[i].Target != null)
                    {
                        if (!bulletDataList[i].IsTargetReached)
                        {
                            var scaleY = bulletDataList[i].Target.transform.lossyScale.y;
                            var scaleX = bulletDataList[i].Target.transform.lossyScale.x;
                            var offset = new Vector3(0, 40, 0);
                            var distance = ExtendedMonoBehaviour.CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.transform.position + offset);
                            
                            if (distance > 30)
                            {
                                bulletList[i].transform.LookAt(bulletDataList[i].Target.transform.position + offset);
                                bulletList[i].transform.Translate(Vector3.forward * bulletDataList[i].Speed, Space.Self);
                            }
                            else
                            {
                                HitTarget(bulletDataList[i]);
                            }
                        }                      
                    }
                    else
                    {
                        SetTargetReached(bulletDataList[i]);
                    }
                }
            }
        }

        public bool CheckAllBulletInactive()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].activeSelf)
                {
                    return false;
                }
            }
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

        private void HitTarget(BulletSystem bullet)
        {
            var isChainShot =
                bullet.ChainshotCount > 0 &&
                bullet.RemainingBounceCount > 0;

            if (bullet.AOEShotRange > 0)
            {
                ownerTower.specialSystem.DamageInAOE(bullet);
            }
            else
            {
                bullet.Target.GetComponent<Creep.CreepSystem>().GetDamage(ownerTower.StatsSystem.Stats.Damage, ownerTower);
            }

            if (isChainShot)
            {
                ownerTower.specialSystem.SetChainTarget(bullet);
            }
            else
            {
                SetTargetReached(bullet);
            }
        }

        private void ShotBullet()
        {
            var shotCount = ownerTower.specialSystem.CalculateShotCount();

            for (int i = 0; i < shotCount; i++)
            {
                CreateBullet(ownerTower.RangeSystem.CreepList[i]);
            }          
        }
      
        protected class ShootState : IState
        {
            private TowerCombatSystem owner;

            public ShootState(TowerCombatSystem owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {                                   
            }

            public void Execute()
            {
                owner.timer += Time.deltaTime;
                owner.MoveBullet();

                if (owner.timer > owner.ownerTower.StatsSystem.Stats.AttackSpeed)
                {
                    owner.ShotBullet();
                    owner.timer = 0;
                }     
            }

            public void Exit()
            {
               
            }
        }
    }
}
