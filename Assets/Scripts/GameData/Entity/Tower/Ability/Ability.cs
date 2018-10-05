using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Base Ability")]
    
    public class Ability : ScriptableObject
    {
        public string AbilityName, AbilityDescription;

        public float ManaCost, Cooldown, TriggerChance;
        
        public List<Effect.Effect> EffectList;

        public List<GameObject> creep, tower;
        public Creep.CreepSystem creepData;
        private int effectCount;
        private bool isEffectCooldown;


        public void Awake()
        {

            creep = new List<GameObject>();
            tower = new List<GameObject>();
            effectCount = 0;
        }

        public void GetData()
        {
           

            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i].creepData = creepData;

                for (int j = 0; j < creep.Count; j++)
                {
                    EffectList[i].creep.Add(creep[j]);
                }

                for (int j = 0; j < tower.Count; j++)
                {
                    EffectList[i].tower.Add(tower[j]);
                }
            }
        }

        public void InitAbility()
        {
            if (effectCount < EffectList.Count)
            {
                if (!isEffectCooldown)
                {
                    Debug.Log(effectCount);
                    GetData();
                    isEffectCooldown = true;
                    EffectList[effectCount].InitEffect();
                    GameManager.Instance.StartCoroutine(NextEffectInterval(EffectList[effectCount].NextEffectInterval));
                }

            }
            else
            {
                effectCount = 0;
            }
        }

        private IEnumerator NextEffectInterval(float delay)
        {
            yield return new WaitForSeconds(delay);
            isEffectCooldown = false;

            effectCount++;
        }
    }


}