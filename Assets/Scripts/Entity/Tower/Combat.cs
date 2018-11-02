using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;


namespace Game.Tower.System
{   
    [Serializable]
    public class Combat 
    {
        public StateMachine State;
        
        public bool isHaveChainTargets;

        private List<BulletSystem> bulletDataList;
        private List<GameObject> bulletList;
        private TowerSystem tower;
        private ObjectPool bulletPool;
        private float timer;

        public Combat(TowerSystem ownerTower) => tower = ownerTower;

        private void OnDestroy() => bulletPool.DestroyPool();

        public void Set()
        {
            bulletList = new List<GameObject>();
            bulletDataList = new List<BulletSystem>();

            bulletPool = new ObjectPool();
            bulletPool.poolObject = tower.Bullet;
            bulletPool.parent = tower.transform;
            bulletPool.Initialize();

            State = new StateMachine();
            State.ChangeState(new ShootState(this));
            timer = tower.GetStats().AttackSpeed;
        }

        private void CreateBullet(Creep.CreepSystem target)
        {
            bulletList.Add(bulletPool.GetObject());
            bulletDataList.Add(bulletList[bulletList.Count - 1].GetComponent<BulletSystem>());

            bulletDataList[bulletDataList.Count - 1].Target = target.gameObject;
            bulletList[bulletList.Count - 1].transform.position = tower.ShootPointTransform.position;
            bulletList[bulletList.Count - 1].transform.rotation = tower.MovingPartTransform.rotation;

            bulletList[bulletList.Count - 1].SetActive(true);
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
                    if (bulletDataList[i].Target != null)
                        if (bulletDataList[i].IsTargetReached)
                            SetTargetReached(bulletDataList[i]);
                        else
                        {
                            var offset = new Vector3(0, 40, 0);
                            var distance = ExtendedMonoBehaviour.CalcDistance(bulletList[i].transform.position, bulletDataList[i].Target.transform.position + offset);

                            if (distance < 30)
                                HitTarget(bulletDataList[i]);
                            else
                            {
                                bulletList[i].transform.LookAt(bulletDataList[i].Target.transform.position + offset);
                                bulletList[i].transform.Translate(Vector3.forward * bulletDataList[i].Speed, Space.Self);
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

        private void HitTarget(BulletSystem bullet)
        {
            var isChainShot =
                bullet.ChainshotCount > 0 &&
                bullet.RemainingBounceCount > 0;

            if (bullet.AOEShotRange > 0)
                tower.GetSpecial().DamageInAOE(bullet);
            else
                bullet.Target.GetComponent<Creep.CreepSystem>().GetDamage(tower.GetStats().Damage.Value, tower);            

            if (isChainShot)
                tower.GetSpecial().SetChainTarget(bullet);
            else           
                SetTargetReached(bullet);          
        }

        private void ShotBullet()
        {
            var shotCount = tower.GetSpecial().CalculateShotCount();

            for (int i = 0; i < shotCount; i++)
                CreateBullet(tower.GetCreepInRangeList()[i]);                    
        }
      
        protected class ShootState : IState
        {
            private readonly Combat o;

            public ShootState(Combat o) => this.o = o;

            public void Enter() { }

            public void Execute()
            {
                o.timer += Time.deltaTime;
                o.MoveBullet();

                if (o.timer > o.tower.GetStats().AttackSpeed)
                {
                    o.ShotBullet();
                    o.timer = 0;
                }     
            }

            public void Exit() { }
        }
    }
}
