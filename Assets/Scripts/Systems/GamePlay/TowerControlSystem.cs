// using System.Collections;
// using System.Collections.Generic;
// using Game.Tower;
// using UnityEngine;

// namespace Game.Systems
// {
// 	public class TowerControlSystem 
// 	{		
// 		private List<StateMachine> stateMachineList;
// 		private List<TowerSystem> towerSystemList;

// 		public void OnTowerCreated(GameObject tower)
// 		{
// 			towerSystemList.Add(new TowerSystem(tower));
// 			stateMachineList.Add(new StateMachine());
// 			stateMachineList[stateMachineList.Count - 1].ChangeState(new SpawnState(towerSystemList[towerSystemList.Count - 1]));
// 		}		
		
// 		private void SetTowerColor(Color color)
//         {
//             for (int i = 0; i < rendererList.Length; i++)
//                 rendererList[i].material.color = color;
//         }

// 		protected class SpawnState : IState
//         {
//             private readonly TowerSystem o;

//             public SpawnState(TowerSystem o) => this.o = o;

//             public void Enter() { }

//             public void Execute()
//             {
//                 void StartPlacing()
//                 {
//                     o.SetTowerColor(GM.I.TowerPlaceSystem.GhostedTowerColor);
//                     o.transform.position = GM.I.TowerPlaceSystem.GhostedTowerPos;
//                 }

//                 if (GM.PlayerState == State.PlacingTower)
//                     StartPlacing();
//                 else
//                     o.state.ChangeState(new LookForCreepState(o));
//             }

//             public void Exit() 
//             {
//                 void EndPlacing()
//                 {
//                     o.transform.position = o.ocuppiedCell.transform.position;

//                     o.SetTowerColor(Color.white - new Color(0.2f, 0.2f, 0.2f));

//                     var placeEffect = Instantiate(GM.I.ElementPlaceEffectList[(int)o.Stats.Element],
//                         o.transform.position + Vector3.up * 5,
//                         Quaternion.identity);

//                     Destroy(placeEffect, placeEffect.GetComponent<ParticleSystem>().main.duration);

//                     o.gameObject.layer = 14;
//                     o.rangeSystem.SetShow(false);

//                     GM.I.PlayerInputSystem.NewTowerData = null;
//                     o.isTowerPlaced = true;
//                 }

//                 EndPlacing();  
//             }          
//         }

//         protected class LookForCreepState : IState
//         {
//             private readonly TowerSystem o;

//             public LookForCreepState(TowerSystem o) => this.o = o;

//             public void Enter() { }

//             public void Execute()
//             {
//                 if (o.rangeSystem.CreepList.Count > 0)
//                     o.state.ChangeState(new CombatState(o));
//             }

//             public void Exit() { }
//         }

//         protected class CombatState : IState
//         {
//             private readonly TowerSystem o;

//             public CombatState(TowerSystem o) => this.o = o;

//             public void Enter() { }

//             public void Execute()
//             {              
//                 o.combatSystem.State.Update();

//                 for (int i = 0; i < o.GetCreepInRangeList().Count; i++)
//                     if (o.GetCreepInRangeList()[i] == null)
//                     {
//                         o.rangeSystem.CreepList.RemoveAt(i);
//                         o.rangeSystem.CreepSystemList.RemoveAt(i);
//                     }

//                 if (o.GetCreepInRangeList().Count < 1)
//                     o.state.ChangeState(new MoveRemainingBulletState(o));
//                 else
//                     if (o.GetCreepInRangeList()[0] != null)
//                     {
//                         o.target = o.GetCreepInRangeList()[0].gameObject;
//                         RotateAtCreep();
//                     }

//                 void RotateAtCreep()
//                 {
//                     var offset = o.target.transform.position - o.transform.position;
//                     offset.y = 0;
//                     o.movingPartTransform.rotation = Quaternion.Lerp(o.movingPartTransform.rotation, 
//                                                                     Quaternion.LookRotation(offset), 
//                                                                     Time.deltaTime * 9f);
//                 }
//             }

//             public void Exit() { }
//         }

//         protected class MoveRemainingBulletState : IState
//         {
//             private readonly TowerSystem o;

//             public MoveRemainingBulletState(TowerSystem o) => this.o = o;

//             public void Enter() { }

//             public void Execute()
//             {
//                 if (o.rangeSystem.CreepList.Count > 0)
//                     o.state.ChangeState(new CombatState(o));
//                 else
//                 if (!o.combatSystem.CheckAllBulletInactive())
//                     o.combatSystem.MoveBullet();
//                 else
//                     o.state.ChangeState(new LookForCreepState(o));
//             }

//             public void Exit() { }
//         }
// 	}

	
// }