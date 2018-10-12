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
    private int abilityStackRequiredIndex, stackCounter;

    protected override void Awake()
    {
        if ((object)CachedTransform == null)
        {
            CachedTransform = transform;
        }       

        towerBaseSystem = GetComponent<TowerBaseSystem>();
        stackedAbilityList = new List<Game.Data.Ability>();

        state = new StateMachine();
        state.ChangeState(new LookForCreepState(this));
    }

    protected class LookForCreepState : IState
    {
        TowerAbilitySystem owner;

        public LookForCreepState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            Debug.Log("Look For Creep State");
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

    protected class CreateStackAbilityState : IState
    {
        TowerAbilitySystem owner;

        public CreateStackAbilityState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            var tower = owner.towerBaseSystem;
            var abilityList = tower.Stats.AbilityList;


            
            owner.state.ChangeState(new CombatState(owner));
        }

        public void Execute()
        {         
        }

        public void Exit()
        {
        }
    }


    protected class CombatState: IState
    {
        TowerAbilitySystem owner;

        public CombatState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            Debug.Log("Combat State");
        }

        public void Execute()
        {
            var tower = owner.towerBaseSystem;
            var abilityList = owner.towerBaseSystem.Stats.AbilityList;

            var isCreepInRange =
                tower.RangeSystem.CreepInRangeList.Count > 0 &&
                tower.RangeSystem.CreepInRangeList[0] != null;

            if (isCreepInRange)
            {

                for (int i = 0; i < abilityList.Count; i++)
                {
                    abilityList[i].SetTarget(tower.RangeSystem.CreepInRangeSystemList);

                    abilityList[i].InitAbility();

                    if (abilityList[i].IsStackable && !owner.isStackRequired)
                    {
                        if (!abilityList[i].IsAbilityOnCooldown && !abilityList[i].CheckAllEffectsEnded())
                        {
                            Debug.Log("Combat Ssastate");
                            if (owner.stackedAbilityList.Count > 0)
                            {
                                if (!owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].CheckAllEffectsEnded())
                                {
                                    owner.abilityStackRequiredIndex = i;
                                    owner.isStackRequired = true;
                                }
                            }
                            else
                            {
                                owner.abilityStackRequiredIndex = i;
                                owner.isStackRequired = true;
                            }
                        }
                    }
                }
                 
                if (owner.isStackRequired)
                {
                    owner.stackedAbilityList.Add(Instantiate(abilityList[owner.abilityStackRequiredIndex]));
                    owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].StackReset();                  
                    owner.isStackRequired = false;
                    //Debug.Log("Stacked: " + owner.stackedAbilityList.Count);
                }

                if (owner.stackedAbilityList.Count > 0)
                {
                    Debug.Log("Stacked: " + owner.stackedAbilityList.Count);

                    for (int i = 0; i < owner.stackedAbilityList.Count; i++)
                    {
                        owner.stackedAbilityList[i].SetTarget(tower.RangeSystem.CreepInRangeSystemList);

                        owner.stackedAbilityList[i].InitAbility();

                        if (owner.stackedAbilityList[i].CheckAllEffectsEnded())
                        {
                           // Debug.Log("Destroyed: " + owner.stackedAbilityList[i]);
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
                    if (!abilityList[i].CheckAllEffectsEnded())
                    {
                        owner.state.ChangeState(new ContinueEffectState(owner));
                    }                       
                }

                if (owner.stackedAbilityList.Count > 0)
                {
                    for (int i = 0; i < owner.stackedAbilityList.Count; i++)
                    {
                        if (!owner.stackedAbilityList[i].CheckAllEffectsEnded())
                        {
                            owner.state.ChangeState(new ContinueEffectState(owner));
                        }
                    }
                }

                Debug.Log("Going to Look For Creep State");
                owner.state.ChangeState(new LookForCreepState(owner));
            }
        }

        public void Exit()
        {
        }
    }



    protected class ContinueEffectState : IState
    {
        TowerAbilitySystem owner;

        public ContinueEffectState(TowerAbilitySystem owner)
        {
            this.owner = owner;
        }

        public void Enter()
        {
            Debug.Log("Continu Effect State");
        }

        public void Execute()
        {
            var tower = owner.towerBaseSystem;
            var abilityList = tower.Stats.AbilityList;

            var isCreepInRange =
                tower.RangeSystem.CreepInRangeList.Count > 0 &&
                tower.RangeSystem.CreepInRangeList[0] != null;

            if ((owner.isAllEffectsEnded && owner.isAllStackedEffectsEnded) || isCreepInRange)
            {
                owner.state.ChangeState(new CombatState(owner));
            }
            else
            {
                owner.isAllEffectsEnded = true;

                for (int i = 0; i < abilityList.Count; i++)
                {
                    abilityList[i].InitAbility();

                    owner.isAllEffectsEnded = abilityList[i].CheckAllEffectsEnded();
                }


                if (owner.stackedAbilityList.Count > 0)
                {
                    for (int i = 0; i < owner.stackedAbilityList.Count; i++)
                    {
                        owner.stackedAbilityList[i].InitAbility();

                        owner.isAllStackedEffectsEnded = owner.stackedAbilityList[i].CheckAllEffectsEnded();

                        if (owner.isAllStackedEffectsEnded)
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
