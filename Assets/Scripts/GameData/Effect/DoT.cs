using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;



namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "DoT", menuName = "Base DoT")]
    public class DoT : Effect, IEffect
    {

        private int counter;

        private void Awake()
        {
            isStackable = false;
        }

        public override void InitEffect()
        {
            if (!isSet)
            {

                GameManager.Instance.StartCoroutine(SetEffect(1f));

            }

            if (!isEnded)
            {
                if(affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] == null)
                    {
                        GameManager.Instance.StopAllCoroutines();
                        affectedCreepDataList.RemoveAt(0);
                        counter = 0;
                        isSet = false;
                        isEnded = true;
                    }
                }
            }
        }

        public IEnumerator SetEffect(float delay)
        {
            if (creepDataList.Count > 0)
            {                
                isSet = true;
                isEnded = false;

                affectedCreepDataList.Add(creepDataList[0]);

                affectedCreepDataList[0].GetComponent<Renderer>().material.color = Color.green;

                while (counter < Duration && affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] != null)
                    {
                        affectedCreepDataList[0].GetDamage(40);                       
                    }
                    else
                    {
                        break;
                    }
                    
                    counter++;
                    yield return new WaitForSeconds(delay);                   
                }

                if (affectedCreepDataList.Count > 0)
                {
                    if (affectedCreepDataList[0] != null)
                    {
                        affectedCreepDataList[0].GetComponent<Renderer>().material.color = Color.white;
                    }
                }

                affectedCreepDataList.RemoveAt(0);
                counter = 0;
                isSet = false;
                isEnded = true;
            }
        }
    }
}