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

    public class CreateStackAbilityState : IState
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

            Debug.Log("CreateStackAbilityState");
            var ability = Instantiate(abilityList[owner.abilityStackRequiredIndex]);
            owner.stackedAbilityList.Add(ability);
            owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].StackReset();
            


            for (int i = 0; i < owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].EffectList.Count; i++)
            {
                owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].EffectList[i] = 
                    Instantiate(owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].EffectList[i]);

                owner.stackedAbilityList[owner.stackedAbilityList.Count - 1].EffectList[i].StackReset();
            }

            owner.isStackRequired = false;

            Debug.Log(owner.stackedAbilityList.Count);
            owner.state.ChangeState(new CombatState(owner));
        }

        public void Execute()
        {         
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
                   
                    if (!abilityList[i].IsAbilityOnCooldown && abilityList[i].IsStackable)
                    {
                        if (abilityList[i].EffectCount < abilityList[i].EffectList.Count - 1)
                        {
                            owner.abilityStackRequiredIndex = i;
                            owner.state.ChangeState(new CreateStackAbilityState(owner));
                        }

                        for (int j = 0; j < abilityList[i].EffectList.Count; j++)
                        {
                            if (!abilityList[i].EffectList[j].IsEnded)
                            {
                                owner.abilityStackRequiredIndex = i;
                                owner.state.ChangeState(new CreateStackAbilityState(owner));
                                
                            }
                        }

                       
                    }
                   
                    //Debug.Log("Processing abiltiy;");
                }

                if (owner.isStackRequired)
                {
                    
                    //Debug.Log("Stacking ability..");

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
                            else
                            {
                                //Destroy(owner.stackedAbilityList[i].EffectList[j]);
                                //owner.stackedAbilityList[i].EffectList.RemoveAt(j);
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
                Debug.Log("No creep in range");
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
                Debug.Log("Going to Look For Creep State");
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
                        owner.stackedAbilityList[i].InitAbility();

                        for (int j = 0; j < owner.stackedAbilityList[i].EffectList.Count; j++)
                        {
                            owner.isAllStackedEffectsEnded = true;

                            if (!owner.stackedAbilityList[i].EffectList[j].IsEnded)
                            {
                                owner.isAllStackedEffectsEnded = false;
                            }
                            else
                            {
                                Destroy(owner.stackedAbilityList[i].EffectList[j]);
                                owner.stackedAbilityList[i].EffectList.RemoveAt(j);
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
        }

        public void Exit()
        {
        }
    }

}
