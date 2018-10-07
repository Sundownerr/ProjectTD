using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect
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

            ContinueEffect();
        }

        public override void StartEffect()
        {
            if (creepDataList.Count > 0)
            {
                affectedCreepDataList.Add(creepDataList[0]);

                if (affectedCreepDataList.Count > 0)
                {
                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] != null)
                        {
                            targetMoveSpeed = affectedCreepDataList[i].Stats.moveSpeed;

                            prefab = Instantiate(TestPrefab, affectedCreepDataList[i].transform.position, Quaternion.Euler(0, 0, 0), affectedCreepDataList[i].transform);

                            affectedCreepDataList[i].Stats.moveSpeed = 0;
                        }
                    }                   

                    isSet = true;
                    isEnded = false;
                }
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
                for (int i = 0; i < affectedCreepDataList.Count; i++)
                {
                    if (affectedCreepDataList[i] != null)
                    {
                        affectedCreepDataList[i].Stats.moveSpeed = targetMoveSpeed;
                       
                    }
                }
                affectedCreepDataList.RemoveAt(0);
            }
            else
            {
                GameManager.Instance.StopCoroutine(SetEffect(Duration));
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
