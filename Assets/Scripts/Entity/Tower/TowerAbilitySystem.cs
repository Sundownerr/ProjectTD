using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tower;

public class TowerAbilitySystem : ExtendedMonoBehaviour
{
    public StateMachine state;

    private TowerBaseSystem towerBaseSystem;
    private List<Game.Data.Ability> stackedAbilityList;
    private bool isAllEffectsEnded, isAllStackedEffectsEnded, isStackRequired;

    private void Start()
    {
        state = new StateMachine();
        state.ChangeState(new LookForCreepState(this));

        towerBaseSystem = GetComponent<TowerBaseSystem>();
        stackedAbilityList = new List<Game.Data.Ability>();
    }

    public class LookForCreepState : IState
    {
        TowerAbilitySystem owner;

        public LookForCreepState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
        }

        public void Execute()
        {
            var tower = owner.towerBaseSystem;

            var isCreepInRange =
                tower.RangeSystem.CreepInRangeList.Count > 0 &&
                tower.RangeSystem.CreepInRangeList[0] != null;

            if (isCreepInRange)
            {
                owner.state.ChangeState(new CombatState(owner));
            }
        }

        public void Exit()
        {
        }
    }


    public class CombatState: IState
    {
        TowerAbilitySystem owner;

        public CombatState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
        }

        public void Execute()
        {
            var tower = owner.towerBaseSystem;
            var abilityList = tower.Stats.AbilityList;

            var isCreepInRange = 
                tower.RangeSystem.CreepInRangeList.Count > 0 && 
                tower.RangeSystem.CreepInRangeList[0] != null;

            if (isCreepInRange)
            {
               
                for (int i = 0; i < abilityList.Count; i++)
                {
                    abilityList[i].SetTarget(tower.RangeSystem.CreepInRangeSystemList);

                    abilityList[i].InitAbility();

                    owner.isStackRequired = false;

                    if (!abilityList[i].IsAbilityOnCooldown && abilityList[i].IsStackable)
                    {
                        for (int j = 0; j < abilityList[i].EffectList.Count; j++)
                        {
                            if (!abilityList[i].EffectList[j].IsEnded)
                            {
                                owner.isStackRequired
                                owner.stackedAbilityList.Add(Instantiate(abilityList[i]));
                                
                                break;
                            }
                        }
                    }
                }

                if (owner.stackedAbilityList.Count > 0)
                {
                    for (int i = 0; i < owner.stackedAbilityList.Count; i++)
                    {
                        owner.stackedAbilityList[i].SetTarget(tower.RangeSystem.CreepInRangeSystemList);

                        owner.stackedAbilityList[i].InitAbility();

                        for (int j = 0; j < owner.stackedAbilityList[i].EffectList.Count; j++)
                        {
                            owner.isAllStackedEffectsEnded = true;

                            if (!owner.stackedAbilityList[i].EffectList[j].IsEnded)
                            {
                                owner.isAllStackedEffectsEnded = false;
                            }
                        }

                        if (owner.isAllStackedEffectsEnded)
                        {
                            Destroy(owner.stackedAbilityList[i]);
                            owner.stackedAbilityList.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < abilityList.Count; i++)
                {
                    for (int j = 0; j < abilityList[i].EffectList.Count; j++)
                    {
                        if (!abilityList[i].EffectList[j].IsEnded)
                        {
                            owner.state.ChangeState(new ContinueEffectState(owner));
                        }
                    }
                }

                if (owner.stackedAbilityList.Count > 0)
                {
                    for (int i = 0; i < owner.stackedAbilityList.Count; i++)
                    {
                        for (int j = 0; j < owner.stackedAbilityList[i].EffectList.Count; j++)
                        {
                            if (!owner.stackedAbilityList[i].EffectList[j].IsEnded)
                            {
                                owner.state.ChangeState(new ContinueEffectState(owner));
                            }
                        }
                    }
                }

                owner.state.ChangeState(new LookForCreepState(owner));
            }
        }

        public void Exit()
        {
        }
    }

    

    public class ContinueEffectState : IState
    {
        TowerAbilitySystem owner;

        public ContinueEffectState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
        }

        public void Execute()
        {
            var tower = owner.towerBaseSystem;
            var abilityList = tower.Stats.AbilityList;

            var isCreepInRange =
                tower.RangeSystem.CreepInRangeList.Count > 0 &&
                tower.RangeSystem.CreepInRangeList[0] != null;

            if (owner.isAllEffectsEnded || isCreepInRange)
            {
                owner.state.ChangeState(new CombatState(owner));
            }
            else
            {
                for (int i = 0; i < abilityList.Count; i++)
                {
                    abilityList[i].InitAbility();

                    for (int j = 0; j < abilityList[i].EffectList.Count; j++)
                    {
                        owner.isAllEffectsEnded = true;

                        if (!abilityList[i].EffectList[j].IsEnded)
                        {
                            owner.isAllEffectsEnded = false;
                        }
                    }
                }

                if (owner.stackedAbilityList.Count > 0)
                {
                    for (int i = 0; i < owner.stackedAbilityList.Count; i++)
                    {
                        for (int j = 0; j < owner.stackedAbilityList[i].EffectList.Count; j++)
                        {
                            owner.isAllStackedEffectsEnded = true;

                            if (!owner.stackedAbilityList[i].EffectList[j].IsEnded)
                            {
                                owner.isAllStackedEffectsEnded = false;
                            }                         
                        }

                        if (owner.isAllEffectsEnded)
                        {
                            Destroy(owner.stackedAbilityList[i]);
                            owner.stackedAbilityList.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void Exit()
        {
        }
    }

}
