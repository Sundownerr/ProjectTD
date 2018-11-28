using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class AbilityControlSystem : ITowerSystem
    {
        private TowerSystem tower;
        private List<AbilitySystem> abilitySystemList, abilityStackList;
        private bool isAllEffectsEnded, isInContinueState;

        public AbilityControlSystem(TowerSystem ownerTower)
        {
            tower = ownerTower;
        }

        public void Set()
        {
            abilitySystemList = tower.AbilitySystemList;
            abilityStackList = new List<AbilitySystem>();
        }

        public void UpdateSystem()
        {
            if (tower.CreepInRangeList.Count > 0)
            {
                isInContinueState = false;

                for (int i = 0; i < abilitySystemList.Count; i++)
                    {
                        if (abilitySystemList[i].IsNeedStack)
                            CreateStack(i);

                        Init(abilitySystemList[i], CheckTargetInRange(abilitySystemList[i].Target));
                    }

                for (int i = 0; i < abilityStackList.Count; i++)
                    Init(abilityStackList[i], !abilityStackList[i].CheckAllEffectsEnded());
            }
            else
            {
                isAllEffectsEnded = true;

                for (int i = 0; i < abilitySystemList.Count; i++)
                    CheckContinueEffects(abilitySystemList[i]);

                for (int i = 0; i < abilityStackList.Count; i++)
                    CheckContinueEffects(abilityStackList[i]);

                if (!isAllEffectsEnded)
                    ContinueEffects();
            }

            void CheckContinueEffects(AbilitySystem abilitySystem)
            {
                if (!abilitySystem.CheckAllEffectsEnded())
                    isAllEffectsEnded = false;
            }
            
            void CreateStack(int index)
            {
                var stack =  new AbilitySystem(tower.Stats.AbilityList[index], tower);

                stack.StackReset(tower);
                stack.SetTarget(abilitySystemList[index].Target);

                abilityStackList.Add(stack);
                abilitySystemList[index].IsNeedStack = false;
            }

            void ContinueEffects()
            {        
                isInContinueState = true;

                for (int i = 0; i < abilitySystemList.Count; i++)
                    Init(abilitySystemList[i], !abilitySystemList[i].CheckAllEffectsEnded());

                for (int i = 0; i < abilityStackList.Count; i++)
                    Init(abilityStackList[i], !abilityStackList[i].CheckAllEffectsEnded());              
            }
        }

        private bool CheckTargetInRange(EntitySystem target)
        {
            for (int i = 0; i < tower.CreepInRangeList.Count; i++)
                if (target == tower.CreepInRangeList[i])
                    return true;
            return false;
        }

        private void Init(AbilitySystem abilitySystem, bool condition)
        {
            if (abilitySystem.Target != null && condition)
            {
                isAllEffectsEnded = false;
                abilitySystem.Init();
            }
            else
            {
                if (!abilitySystem.IsStacked)
                    if (!isInContinueState)
                        abilitySystem.SetTarget(tower.CreepInRangeList[0]);
                    else
                    {
                        abilitySystem.CooldownReset();
                        abilitySystem.SetTarget(null);
                    }
                else
                {
                    for (int i = 0; i < abilitySystem.EffectSystemList.Count; i++)
                        abilitySystem.EffectSystemList.Remove(abilitySystem.EffectSystemList[i]);

                    abilitySystem.EffectSystemList.Clear();
                    abilityStackList.Remove(abilitySystem);
                }

                isAllEffectsEnded = true;
            }
        }
    }
}