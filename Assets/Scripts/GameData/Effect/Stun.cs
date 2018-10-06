using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect, IEffect
    {       
        private float targetMoveSpeed;
        public GameObject TestPrefab;

        private GameObject prefab;

        public override void InitEffect()
        {
            if (!isSet)
            {
                GameManager.Instance.StartCoroutine(SetEffect(Duration));
            }
        }

        public override void StartEffect()
        {
            if (creepDataList.Count > 0)
            {
                affectedCreepDataList.Add(creepDataList[0]);

                if (affectedCreepDataList[0] != null)
                {
                    targetMoveSpeed = affectedCreepDataList[0].Stats.moveSpeed;
                    prefab = Instantiate(TestPrefab, affectedCreepDataList[0].transform.position + Vector3.down * 30, Quaternion.Euler(0, 0, 0), affectedCreepDataList[0].transform);
                }

                affectedCreepDataList[0].Stats.moveSpeed = 0;

                isSet = true;
                isEnded = false;
            }
        }

        public override void ContinueEffect()
        {
            if (!isEnded)
            {
                if (affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] == null)
                    {
                        GameManager.Instance.StopCoroutine(SetEffect(Duration));
                        affectedCreepDataList.RemoveAt(0);
                        Destroy(prefab);
                        isSet = false;
                        isEnded = true;
                    }
                }
            }
        }

        public override void EndEffect()
        {

            if (affectedCreepDataList.Count > 0)
            {
                if (affectedCreepDataList[0] != null)
                {
                    affectedCreepDataList[0].Stats.moveSpeed = targetMoveSpeed;
                    affectedCreepDataList.RemoveAt(0);
                }
            }
            Destroy(prefab);


            isSet = false;
            isEnded = true;
        }

        public IEnumerator SetEffect(float delay)
        {
            StartEffect();

            yield return new WaitForSeconds(delay);

            EndEffect();
        }

       
    }
}
