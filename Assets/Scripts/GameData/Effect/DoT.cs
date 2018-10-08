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

        public override void InitEffect()
        {
            if (!IsSet)
            {
                GameManager.Instance.StartCoroutine(SetEffect(1f));
                StartEffect();
            }

            ContinueEffect();
        }

        public IEnumerator SetEffect(float delay)
        {
            var damageTick = 0;

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
                    EndEffect();
                    break;
                }

                yield return new WaitForSeconds(delay);
            }

            EndEffect();
        }

        public override void StartEffect()
        {
            if (creepDataList.Count > 0)
            {
                affectedCreepDataList.Add(creepDataList[0]);

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
                            affectedCreepDataList[i].creepRenderer.material.color = Color.green;
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

            IsSet = false;
            IsEnded = true;
        }       
    }
}
