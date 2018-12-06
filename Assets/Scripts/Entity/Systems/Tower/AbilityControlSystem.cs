using System.Collections.Generic;
using Game.Data;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class AbilityControlSystem : ITowerSystem
    {
        private TowerSystem tower;
        private List<AbilitySystem> abilityStacks;
        private bool isAllEffectsEnded, isInContinueState;

        public AbilityControlSystem(TowerSystem ownerTower)
        {
            tower = ownerTower;
        }

        public void Set()
        {            
            abilityStacks = new List<AbilitySystem>();
        }

        public void UpdateSystem()
        {
            var abilitySystems = tower.AbilitySystems;

            if (tower.CreepsInRange.Count > 0)
            {
                isInContinueState = false;

                for (int i = 0; i < abilitySystems.Count; i++)
                {
                    if (abilitySystems[i].IsNeedStack)
                        CreateStack(i);
                    Init(abilitySystems[i], CheckTargetInRange(abilitySystems[i].Target));
                }

                for (int i = 0; i < abilityStacks.Count; i++)
                    Init(abilityStacks[i], !abilityStacks[i].CheckAllEffectsEnded());
            }
            else
            {
                isAllEffectsEnded = true;

                for (int i = 0; i < abilitySystems.Count; i++)
                    CheckContinueEffects(abilitySystems[i]);

                for (int i = 0; i < abilityStacks.Count; i++)
                    CheckContinueEffects(abilityStacks[i]);

                if (!isAllEffectsEnded)
                    ContinueEffects();
            }

            #region  Helper functions

            void CheckContinueEffects(AbilitySystem abilitySystem)
            {
                if (!abilitySystem.CheckAllEffectsEnded())
                    isAllEffectsEnded = false;
            }
            
            void CreateStack(int index)
            {
                var stack =  new AbilitySystem(tower.Stats.Abilities[index], tower);

                stack.StackReset(tower);
                stack.SetTarget(abilitySystems[index].Target);

                abilityStacks.Add(stack);
                abilitySystems[index].IsNeedStack = false;
            }

            void ContinueEffects()
            {        
                isInContinueState = true;

                for (int i = 0; i < abilitySystems.Count; i++)
                    Init(abilitySystems[i], !abilitySystems[i].CheckAllEffectsEnded());

                for (int i = 0; i < abilityStacks.Count; i++)
                    Init(abilityStacks[i], !abilityStacks[i].CheckAllEffectsEnded());              
            }

            bool CheckTargetInRange(EntitySystem target)
            {
                for (int i = 0; i < tower.CreepsInRange.Count; i++)
                    if (target == tower.CreepsInRange[i])
                        return true;
                return false;
            }

            void Init(AbilitySystem abilitySystem, bool condition)
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
                            abilitySystem.SetTarget(tower.CreepsInRange[0]);
                        else
                        {
                            abilitySystem.CooldownReset();
                            abilitySystem.SetTarget(null);
                        }
                    else
                    {
                        for (int i = 0; i < abilitySystem.EffectSystems.Count; i++)
                            abilitySystem.EffectSystems.Remove(abilitySystem.EffectSystems[i]);

                        abilitySystem.EffectSystems.Clear();
                        abilityStacks.Remove(abilitySystem);
                    }

                    isAllEffectsEnded = true;
                }
            }
            
            #endregion
        }
    }
}