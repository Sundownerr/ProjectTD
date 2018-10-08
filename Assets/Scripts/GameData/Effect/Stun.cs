using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Stun")]
    public class Stun : Effect
    {               
        public GameObject TestPrefab;

        private List<float> defaultMoveSpeedList;
        private GameObject prefab;

        private void Awake()
        {
            defaultMoveSpeedList = new List<float>();
        }

        public override void InitEffect()
        {
            if (!IsSet)
            {
                GameManager.Instance.StartCoroutine(SetEffect(Duration));
                StartEffect();
            }

            ContinueEffect();
        }

        public IEnumerator SetEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            EndEffect();
        }

        public override void StartEffect()
        {
            if (creepDataList.Count > 0)
            {
                affectedCreepDataList.Add(creepDataList[0]);
                defaultMoveSpeedList.Add(creepDataList[0].Stats.DefaultMoveSpeed);

                if (affectedCreepDataList.Count > 1)
                {
                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] == creepDataList[0])
                        {
                            EndEffect();
                            break;
                        }
                    }
                }

                if (affectedCreepDataList.Count > 0)
                {
                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] != null)
                        {
                            prefab = Instantiate(TestPrefab, affectedCreepDataList[i].transform.position, Quaternion.identity, affectedCreepDataList[i].transform);

                            affectedCreepDataList[i].Stats.MoveSpeed = 0;
                        }
                    }                   

                    IsSet = true;
                    IsEnded = false;
                }
            }
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] == null)
                    {
                        GameManager.Instance.StopCoroutine(SetEffect(Duration));
                        EndEffect();
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
                        affectedCreepDataList[i].Stats.MoveSpeed = defaultMoveSpeedList[i];
                       
                    }
                }
                affectedCreepDataList.RemoveAt(0);
                defaultMoveSpeedList.RemoveAt(0);
            }
   
            Destroy(prefab);

            IsSet = false;
            IsEnded = true;
        }         
    }
}
