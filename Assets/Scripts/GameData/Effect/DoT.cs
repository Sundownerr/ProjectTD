using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;



namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "DoT", menuName = "Base DoT")]
    public class DoT : Effect, IEffect
    {
        private int damageTick, creepCounter;

        public override void InitEffect()
        {
            if (!isSet)
            {
                GameManager.Instance.StartCoroutine(SetEffect(1f));
            }

            ContinueEffect();
        }

        public override void StartEffect()
        {
            affectedCreepDataList.Add(creepDataList[0]);

            if (affectedCreepDataList.Count > 0)
            {
                if (affectedCreepDataList[0] != null)
                {
                    affectedCreepDataList[0].GetComponent<Renderer>().material.color = Color.green;
                }

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
                        GameManager.Instance.StopCoroutine(SetEffect(1f));
                        affectedCreepDataList.RemoveAt(0);
                        EndEffect();
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
                    affectedCreepDataList[0].GetComponent<Renderer>().material.color = Color.white;
                }

                affectedCreepDataList.RemoveAt(0);
            }

            damageTick = 0;
            isSet = false;
            isEnded = true;
        }

        public IEnumerator SetEffect(float delay)
        {
            if (creepDataList.Count > 0)
            {
                StartEffect();

                while (damageTick < Duration && affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] != null)
                    {
                        affectedCreepDataList[0].GetDamage(40);                       
                    }
                    else
                    {
                        EndEffect();
                    }
                    
                    damageTick++;

                    yield return new WaitForSeconds(delay);                   
                }

                EndEffect();
            }
        }

      
    }
}