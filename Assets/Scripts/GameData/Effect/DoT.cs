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
        public GameObject EffectPrefab;

        private float damageTick;
        private GameObject effectPrefab;
        private ParticleSystem[] psList;

        private void Awake()
        {
            
        }

        private void Show(bool enabled)
        {
            
            for (int i = 0; i < psList.Length; i++)
            {
                var emissionModule = psList[i].emission;
                emissionModule.enabled = enabled;

                if (enabled)
                {
                    psList[i].Play();
                 
                }
                else
                {
                    psList[i].Stop();
                }
            }
        }

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
                            effectPrefab = Instantiate(EffectPrefab, 
                                affectedCreepDataList[i].transform.position + Vector3.up * affectedCreepDataList[i].transform.GetChild(0).lossyScale.x / 2, 
                                Quaternion.Euler(0, 0, 0), affectedCreepDataList[i].transform);

                            psList = effectPrefab.GetComponentsInChildren<ParticleSystem>();
                            affectedCreepDataList[i].creepRenderer.material.color = Color.green;
                            Show(true);
                        }
                    }                    

                    IsSet = true;
                    IsEnded = false;

                    GameManager.Instance.StartCoroutine(SetEffect(1f));
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

           
            Destroy(effectPrefab);
            damageTick = 0;

            IsSet = false;
            IsEnded = true;
        }       
    }
}
