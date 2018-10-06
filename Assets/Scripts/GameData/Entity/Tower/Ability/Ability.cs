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
        public List<Effect.Effect> EffectList, StackEffectList;
        public List<GameObject> creepList, towerList;
        public List<Creep.CreepSystem> creepDataList;

        private int effectCount;
        private List<int> stackEffectCount;
        private bool isAbilityCooldown;
        private StateMachine state;

        public void Awake()
        {
            state = new StateMachine();
            state.ChangeState(new ChoseEffectState(this));

            creepList = new List<GameObject>();
            towerList = new List<GameObject>();
            StackEffectList = new List<Effect.Effect>();
            stackEffectCount = new List<int>();
        }

        public void InitAbility()
        {
            state.Update();
        }

        public void SetTarget(List<Creep.CreepSystem> data)
        {
            if (data != null && data.Count > 0 && creepDataList != data)
            {
                creepDataList = data;

                SetEffectData();
            }          
        }

        public void SetEffectData()
        {
            for (int i = 0; i < EffectList.Count; i++)
            {
                EffectList[i].creepDataList = creepDataList;
            }

            if (StackEffectList.Count > 0)
            {
                for (int i = 0; i < StackEffectList.Count; i++)
                {
                    StackEffectList[i].creepDataList = creepDataList;
                }
            }
        }

        private IEnumerator NextEffect(float delay)
        {
            yield return new WaitForSeconds(delay);

            state.ChangeState(new ChoseEffectState(this));
        }

        private IEnumerator AbilityCooldown(float delay)
        {
            Debug.Log(effectCount);
            isAbilityCooldown = true;

            yield return new WaitForSeconds(delay);
                    
            effectCount = 0;
            isAbilityCooldown = false;
        }

        public class ChoseEffectState : IState
        {
            private Ability owner;

            public ChoseEffectState(Ability owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {               
            }

            public void Execute()
            {
                if (!owner.isAbilityCooldown)
                {
                    if (owner.effectCount < owner.EffectList.Count || owner.stackEffectCount[owner.StackEffectList.Count - 1] < owner.StackEffectList.Count)
                    {
                        owner.state.ChangeState(new SetEffectState(owner));
                    }
                }
            }          

            public void Exit()
            {               
                GameManager.Instance.StartCoroutine(owner.AbilityCooldown(owner.Cooldown));

                if(owner.StackEffectList.Count > 0)
                {
                    for (int i = 0; i < owner.StackEffectList.Count; i++)
                    {
                        if (owner.StackEffectList[i].isEnded)
                        {
                            Destroy(owner.StackEffectList[i]);
                            owner.StackEffectList.RemoveAt(i);
                            owner.stackEffectCount.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public class SetEffectState : IState
        {
            private Ability owner;

            public SetEffectState(Ability owner)
            {
                this.owner = owner;
            }

            public void Enter()
            {
                GameManager.Instance.StartCoroutine(owner.NextEffect(owner.EffectList[owner.effectCount].NextEffectInterval));

                if (owner.StackEffectList.Count > 0)
                {
                    for (int i = 0; i < owner.StackEffectList.Count; i++)
                    {
                        GameManager.Instance.StartCoroutine(owner.NextEffect(owner.StackEffectList[owner.stackEffectCount[owner.StackEffectList.Count - 1]].NextEffectInterval));
                    }
                }             
            }

            public void Execute()
            {
                owner.EffectList[owner.effectCount].InitEffect();

                if(owner.StackEffectList.Count > 0)
                {
                    owner.StackEffectList[owner.stackEffectCount[owner.StackEffectList.Count - 1]].InitEffect();
                }
            }

            public void Exit()
            {
                if (!owner.EffectList[owner.effectCount].isEnded)
                {
                    if (owner.EffectList[owner.effectCount].isStackable)
                    {
                        owner.StackEffectList.Add(owner.EffectList[owner.effectCount]);
                        owner.stackEffectCount.Add(-1);
                        owner.stackEffectCount[owner.stackEffectCount.Count - 1]++;
                    }
                }
                owner.effectCount++;
            }
        }       
    }
}