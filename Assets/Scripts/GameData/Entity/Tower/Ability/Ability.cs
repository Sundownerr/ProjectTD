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
        private bool isEffectCooldown, isAbilityCooldown;
        private StateMachine state;

        public void Awake()
        {
            state = new StateMachine();
            state.ChangeState(new ChoseEffectState(this));
            creep = new List<GameObject>();
            tower = new List<GameObject>();
        }

        public void InitAbility()
        {
            state.Update();
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
                    if (owner.effectCount < owner.EffectList.Count)
                    {
                        owner.state.ChangeState(new SetEffectState(owner));
                    }
                }
            }          

            public void Exit()
            {
                owner.GetData();
                GameManager.Instance.StartCoroutine(owner.AbilityCooldown(owner.Cooldown));
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
               
            }

            public void Execute()
            {
                owner.EffectList[owner.effectCount].InitEffect();
            }
            
            public void Exit()
            {
                owner.effectCount++;
            }
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

        private IEnumerator NextEffect(float delay)
        {
            yield return new WaitForSeconds(delay);
            state.ChangeState(new ChoseEffectState(this));
        }
     
        private IEnumerator AbilityCooldown(float delay)
        {
            isAbilityCooldown = true;
            yield return new WaitForSeconds(delay);
            isAbilityCooldown = false;
            effectCount = 0;
        }
    }


}