using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Systems;
using System;
using Game.Creep;
using Game.Tower;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Data/Ability")]

    [Serializable]
    public class Ability : Entity
    {
        public bool IsNeedStack { get => isNeedStack; set => isNeedStack = value; }
        public bool IsOnCooldown { get => isOnCooldown; set => isOnCooldown = value; }
        public EntitySystem Target { get => target; set => target = value; }
        public bool IsStacked { get => isStacked; set => isStacked = value; }

        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect> EffectList;

        private bool isStacked, isNeedStack, isOnCooldown;
        private EntitySystem target;
        private StateMachine state;
        private int effectCount;

        private void Awake()
        {
            state = new StateMachine();           
        }

        public void Init() => state.Update();

        public void SetTarget(EntitySystem target)
        {
            this.target = target;
            SetEffectsTarget(target);
        } 

        private void SetEffectsTarget(EntitySystem target)
        {
            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].SetTarget(target);            
        }

        public override void SetId() 
        {
            base.SetId();

            if (Owner is CreepSystem creep)
            {
                id.AddRange(creep.Stats.Id);   
                id.Add(creep.Stats.AbilityList.IndexOf(this));           
            }
            else if(Owner is TowerSystem tower)
            {
                id.AddRange(tower.Stats.Id);   
                id.Add(tower.Stats.AbilityList.IndexOf(this));
            }
        }       
        
        public void SetOwner(EntitySystem owner)
        {                 
            this.owner = owner;
            SetId();

            for (int i = 0; i < EffectList.Count; i++)           
                EffectList[i].SetOwner(owner, this);
            
            EffectList[EffectList.Count - 1].NextInterval = 0.01f;           
            state.ChangeState(new SetEffectState(this));         
        }

        public void StackReset(EntitySystem owner)
        {
            isStacked = true;

            for (int i = 0; i < EffectList.Count; i++)         
                EffectList[i] = Instantiate(EffectList[i]);              
                               
            SetOwner(owner);  
        }

        public void CooldownReset()
        {
            effectCount = 0;          

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].ApplyRestart();         
        }

        public bool CheckAllEffectsEnded()
        {
            for (int i = 0; i < EffectList.Count; i++)
                if (!EffectList[i].IsEnded)
                    return false;        
            return true;
        }
 
        public bool CheckNeedStack()
        {
            for (int i = 0; i < EffectList.Count; i++)
                if (EffectList[i].IsStackable)
                {
                    if (!EffectList[i].IsEnded)
                        return true;
                }
                else
                    if (EffectList[i].Target != target)
                        return true;

            return false;         
        }

        private IEnumerator StartCooldown(float delay)
        {
            isOnCooldown = true;

            yield return new WaitForSeconds(delay);
           
            isNeedStack = CheckNeedStack();

            CooldownReset();          
            isOnCooldown = false;
        }
     
        protected class SetEffectState : IState
        {
            private readonly Ability o;
            private float nextEffectTimer;

            public SetEffectState(Ability o) => this.o = o; 

            public void Enter() => nextEffectTimer = 0;

            public void Execute()
            {
                nextEffectTimer = nextEffectTimer > o.EffectList[o.effectCount].NextInterval ? 0 : nextEffectTimer += Time.deltaTime;

                for (int i = 0; i <= o.effectCount; i++)
                    o.EffectList[i].Init();  

                if(!o.isStacked && !o.isOnCooldown)
                    GM.I.StartCoroutine(o.StartCooldown(o.Cooldown)); 

                if (!(o.effectCount >= o.EffectList.Count - 1))
                    if (nextEffectTimer > o.EffectList[o.effectCount].NextInterval)                  
                        o.effectCount++;             
            }

            public void Exit() { }
        }        
    }
}