using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data.Effect
{
    [CreateAssetMenu(fileName = "DoT", menuName = "Base DoT")]
    public class DoT : Effect
    {
        public int DamagePerTick;
        private float damageTick, creepCounter;

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
            if (creepDataList.Count > 0)
            {
                if(affectedCreepDataList.Count == 0)
                {
                    affectedCreepDataList.Add(creepDataList[0]);
                }
                else
                {
                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] == creepDataList[0])
                        {
                            EndEffect();
                            break;
                        }
                    }
                            affectedCreepDataList.Add(creepDataList[0]);                        
                    
                }             
                
                if (affectedCreepDataList.Count > 0)
                {
                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] != null)
                        {
                            affectedCreepDataList[i].creepRenderer.material.color = Color.green;
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
                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] == null)
                        {
                            GameManager.Instance.StopCoroutine(SetEffect(1f));
                            EndEffect();
                        }
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
                        affectedCreepDataList[i].creepRenderer.material.color = Color.white;
                    }                    
                }
                affectedCreepDataList.RemoveAt(0);
            }

            damageTick = 0;

            isSet = false;
            isEnded = true;
        }

        public IEnumerator SetEffect(float delay)
        {
            StartEffect();

            while (damageTick < Duration)
            {              
                if (affectedCreepDataList.Count > 0)
                {
                    damageTick++;

                    for (int i = 0; i < affectedCreepDataList.Count; i++)
                    {
                        if (affectedCreepDataList[i] != null)
                        {
                            affectedCreepDataList[i].GetDamage(DamagePerTick);
                        }
                    }
                }
                else
                {
                    damageTick = Duration;
                    EndEffect();
                    break;
                }

                
                yield return new WaitForSeconds(delay);
            }

            EndEffect();
        }
    }
}
