using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Data/Tower/Ability")]

    [Serializable]
    public class Ability : ScriptableObject
    {       
        [HideInInspector]
        public bool IsNeedStack, IsOnCooldown;

        public string AbilityName, AbilityDescription;
        public float Cooldown, TriggerChance;
        public int ManaCost;

        [Expandable]
        public List<Effect.Effect> EffectList;

        private List<Creep.CreepSystem> targetList;
        private Creep.CreepSystem target;
        private bool isStackable, isStacked;
        private StateMachine state;
        private Tower.TowerBaseSystem tower;
        private int effectCount;
        private float timer;

        private void OnEnable()
        {
            state = new StateMachine();
            state.ChangeState(new ChoseEffectState(this));         
        }

        public void Init()
        {
            state.Update();
        }

        public void SetOwnerTower(Tower.TowerBaseSystem oTower)
        {
            tower = oTower;

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].tower = oTower;

            EffectList[EffectList.Count - 1].NextEffectInterval = 0.01f;

            CheckStackable();
        }

        public void SetTarget(Creep.CreepSystem target)
        {
            this.target = target;
            SetEffectsTarget();
        }

        public void GetAvailableTargetList(List<Creep.CreepSystem> targetList)
        {
            var isTargetListOk = 
                targetList.Count > 0 &&
                this.targetList != targetList;

            if (isTargetListOk)
            {
                this.targetList = targetList;

                if (!IsOnCooldown)
                    SetTarget(targetList[0]);
            }
        }

        public Creep.CreepSystem GetTarget()
        {
            return target;
        }

        private void SetEffectsTarget()
        {
            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].SetTarget(target);
        }

        public void EndEffects()
        {
            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].End();           
        }
        
        public void StackReset()
        {
            isStacked = true;

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i] = Instantiate(EffectList[i]);

            state.ChangeState(new SetEffectState(this));
        }

        public void Reset()
        {         
            timer = 0;
            effectCount = 0;
            IsOnCooldown = false;

            for (int i = 0; i < EffectList.Count; i++)
                EffectList[i].Reset();

            if (targetList.Count > 0)
                SetTarget(targetList[0]);
            else
                EndEffects();

            state.ChangeState(new ChoseEffectState(this));
        }

        private void CheckStackable()
        {
            var allEffectsInterval = 0f;
            var allEffectsDuration = 0f;

            for (int i = 0; i < EffectList.Count; i++)
            {
                allEffectsInterval += EffectList[i].NextEffectInterval;
                allEffectsDuration += EffectList[i].Duration;
            }

            isStackable = 
                allEffectsInterval > Cooldown ? true : 
                allEffectsDuration > Cooldown ? true : false;
        }

        public bool CheckEffectsEnded()
        {
            for (int i = 0; i < EffectList.Count; i++)
                if (!EffectList[i].IsEnded)
                    return false;
            
            return true;
        }

        public bool CheckIntervalsEnded()
        {
            return effectCount >= EffectList.Count - 1 ? true : false;         
        }

        private IEnumerator StartCooldown(float delay)
        {
            IsOnCooldown = true;

            yield return new WaitForSeconds(delay);

            IsOnCooldown = false;

            if (isStackable)
                IsNeedStack = !CheckEffectsEnded() ? true : false;
        }

        protected class ChoseEffectState : IState
        {
            private readonly Ability o;

            public ChoseEffectState(Ability o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                if (!o.IsOnCooldown)
                {                         
                    GM.Instance.StartCoroutine(o.StartCooldown(o.Cooldown));
                    o.state.ChangeState(new SetEffectState(o));
                }
            }

            public void Exit() { }
        }

        protected class SetEffectState : IState
        {
            private readonly Ability o;

            public SetEffectState(Ability o) { this.o = o; }

            public void Enter() { }

            public void Execute()
            {
                o.timer += Time.deltaTime;

                for (int i = 0; i <= o.effectCount; i++)
                    o.EffectList[i].Init();

                if (o.CheckIntervalsEnded())
                {
                    if (!o.isStacked)
                        if (!o.IsOnCooldown)
                            o.Reset();
                        else if (o.isStackable)
                            o.IsNeedStack = true;
                }
                else if (o.timer > o.EffectList[o.effectCount].NextEffectInterval)
                {
                    o.effectCount++;
                    o.timer = 0;
                }
            }

            public void Exit()
            {
            }
        }        
    }
}