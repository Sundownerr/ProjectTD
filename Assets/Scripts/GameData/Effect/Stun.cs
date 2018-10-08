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

        private float defaultMoveSpeed;
        private GameObject prefab;

        public override void InitEffect()
        {
            if (!IsSet)
            {
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
                affectedCreepData = creepDataList[0];
                defaultMoveSpeed = creepDataList[0].Stats.DefaultMoveSpeed;

                if (affectedCreepData != null)
                {
                    prefab = Instantiate(TestPrefab, affectedCreepData.transform.position, Quaternion.identity, affectedCreepData.transform);

                    affectedCreepData.Stats.MoveSpeed = 0;
                }
                else
                {
                    EndEffect();
                }

                IsSet = true;
                IsEnded = false;

                GameManager.Instance.StartCoroutine(SetEffect(Duration));
            }
        }

        public override void ContinueEffect()
        {
            if (!IsEnded)
            {
                if (affectedCreepData == null)
                {
                    GameManager.Instance.StopCoroutine(SetEffect(Duration));
                    EndEffect();
                }
            }
        }

        public override void EndEffect()
        {
            if (affectedCreepData != null)
            {
                affectedCreepData.Stats.MoveSpeed = defaultMoveSpeed;
            }

            Destroy(prefab);

            IsSet = false;
            IsEnded = true;
        }
    }
}
