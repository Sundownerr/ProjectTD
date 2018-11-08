using System.Collections;
using System.Collections.Generic;
using Game.Creep;
using UnityEngine;
using Game.Systems;
using Game.Tower;
using Game.Data;

namespace Game.Systems
{
	public class CreepControlSystem 
	{	
        public static void MoveToNextWaypoint(CreepSystem creep)
        {
            if(creep.IsOn && creep != null)
            {
                var waypointTransform = GM.Instance.WaypointList[creep.WaypointIndex].transform;
                var creepTransform = creep.gameObject.transform;
                var waypointReached = ExtendedMonoBehaviour.CalcDistance(creepTransform.position, waypointTransform.position) < 70;

                if (creep.WaypointIndex < GM.Instance.WaypointList.Length - 1)
                    if (!waypointReached)                    
                        MoveAndRotateCreep(creep);                                                     
                    else
                        creep.WaypointIndex++;                    
                else
                    DestroyCreep(creep);
            }
        }

		private static void MoveAndRotateCreep(CreepSystem creep)
        {
           
            var creepTransform = creep.gameObject.transform;
            creepTransform.Translate(Vector3.forward * Time.deltaTime * creep.Stats.MoveSpeed, Space.Self);

            var clampPos = new Vector3(creepTransform.position.x, creepTransform.lossyScale.y, creepTransform.position.z);
            creepTransform.position = clampPos;

            RotateCreep(creep);
        }

        private static void RotateCreep(CreepSystem creep)
        {
            var creepTransform = creep.gameObject.transform;
            var lookRotation = Quaternion.LookRotation(GM.Instance.WaypointList[creep.WaypointIndex].transform.position - creepTransform.position);
            var rotation = Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);
            rotation.z = 0;
            rotation.x = 0;

            creepTransform.localRotation = rotation;
        }

        private static void DestroyCreep(CreepSystem creep)
        {
            GM.Instance.CreepList.Remove(creep.gameObject);
            Object.Destroy(creep.gameObject);
        }

        public static void GiveResources(CreepSystem creep)
        {
             var tower = creep.LastDamageDealer as TowerSystem;

                if (tower != null)
                {
                    tower.AddExp(creep.Stats.Exp);
                    GM.Instance.ResourceSystem.AddGold(creep.Stats.Gold);
                }
              
                DestroyCreep(creep);
        }
	}
}
