using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tower;

public class TowerAbilitySystem : ExtendedMonoBehaviour
{
    public StateMachine state;

    private TowerBaseSystem towerBaseSystem;
    private bool isAllEffectsEnded;

    private void Start()
    {
        state = new StateMachine();
        state.ChangeState(new CombatState(this));

        towerBaseSystem = GetComponent<TowerBaseSystem>();
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
            if (owner.towerBaseSystem.RangeSystem.CreepInRangeList.Count > 0)
            {
                if (owner.towerBaseSystem.RangeSystem.CreepInRangeList[0] != null)
                {
                    for (int i = 0; i < owner.towerBaseSystem.Stats.TowerAbilityList.Count; i++)
                    {
                        owner.towerBaseSystem.Stats.TowerAbilityList[i].SetTarget(owner.towerBaseSystem.RangeSystem.CreepInRangeSystemList);

                        owner.towerBaseSystem.Stats.TowerAbilityList[i].InitAbility();
                    }
                }
            }
            else
            {




                for (int i = 0; i < owner.towerBaseSystem.Stats.TowerAbilityList.Count; i++)
                {
                    if (owner.towerBaseSystem.Stats.TowerAbilityList[i].creepDataList.Count > 0)
                    {
                        for (int j = 0; j < owner.towerBaseSystem.Stats.TowerAbilityList[i].EffectList.Count; j++)
                        {
                            if (!owner.towerBaseSystem.Stats.TowerAbilityList[i].EffectList[j].isEnded)
                            {
                                owner.state.ChangeState(new ContinueEffectState(owner));
                            }
                        }

                        if (owner.towerBaseSystem.Stats.TowerAbilityList[i].StackEffectList.Count > 0)
                        {
                            for (int j = 0; j < owner.towerBaseSystem.Stats.TowerAbilityList[i].StackEffectList.Count; j++)
                            {
                                if (!owner.towerBaseSystem.Stats.TowerAbilityList[i].StackEffectList[j].isEnded)
                                {
                                    owner.state.ChangeState(new ContinueEffectState(owner));
                                }
                            }
                        }
                    }
                }
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
            Debug.Log("asd");
            
            for (int i = 0; i < owner.towerBaseSystem.Stats.TowerAbilityList.Count; i++)
            {
                owner.towerBaseSystem.Stats.TowerAbilityList[i].SetTarget(owner.towerBaseSystem.RangeSystem.CreepInRangeSystemList);

                owner.towerBaseSystem.Stats.TowerAbilityList[i].InitAbility();

                for (int j = 0; j < owner.towerBaseSystem.Stats.TowerAbilityList[i].EffectList.Count; j++)
                {
                    owner.isAllEffectsEnded = true;

                    if(!owner.towerBaseSystem.Stats.TowerAbilityList[i].EffectList[j].isEnded)
                    {
                        owner.isAllEffectsEnded = false;
                    }
                }
            }

            if (owner.isAllEffectsEnded)
            {
                owner.state.ChangeState(new CombatState(owner));
            }
            
        }

        public void Exit()
        {
        }
    }

}
