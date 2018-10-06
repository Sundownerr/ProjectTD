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

        private void Awake()
        {
            isStackable = false;
        }

        public override void InitEffect()
        {
            if (!isSet)
            {
                GameManager.Instance.StartCoroutine(SetEffect(Duration));
            }

            if (!isEnded)
            {
                if (affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] == null)
                    {
                        GameManager.Instance.StopAllCoroutines();
                        isSet = false;
                        isEnded = true;
                    }
                }
            }
        }

        public IEnumerator SetEffect(float delay)
        {
            if (creepDataList[0] != null)
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

                yield return new WaitForSeconds(delay);

                if (affectedCreepDataList[0] != null)
                {
                    affectedCreepDataList[0].Stats.moveSpeed = targetMoveSpeed;
                    affectedCreepDataList.RemoveAt(0);
                }
                    Destroy(prefab);
                

                isSet = false;
                isEnded = true;
            }
        }
    }
}
