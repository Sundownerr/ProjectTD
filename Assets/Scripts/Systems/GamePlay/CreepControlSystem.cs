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
        private List<CreepSystem> creeps = new List<CreepSystem>();
        
        public void SetSystem()
        {
            GM.I.WaveSystem.CreepSpawned += OnCreepSpawned;
        }

        private void OnCreepSpawned(object sender, CreepEventArgs e)
        {
            AddCreep(e.Creep);
        }

        public void AddCreep(CreepSystem creep) 
        {
            creeps.Add(creep);
            GM.I.Creeps.Add(creep);    
            creep.IsOn = true;
            creep.IsVulnerable = true;
        }

        public void UpdateSystem()
        {
            for (int i = 0; i < creeps.Count; i++)
            {
                var creep = creeps[i];             
                
                if (creep == null || creep.Prefab == null)
                    creeps.Remove(creep);
                else
                {
                    if (creep.HealthSystem != null)
                        creep.HealthSystem.Update();             

                    creep.AbilityControlSystem.UpdateSystem();
                    creep.TraitControlSystem.UpdateSystem();
                    
                    if (creep.IsOn)
                    {
                        var waypointTransform = GM.I.Waypoints[creep.WaypointIndex].transform;
                        var creepTransform = creep.Prefab.transform;
                        var waypointReached = QoL.CalcDistance(creepTransform.position, waypointTransform.position) < 70;

                        if (creep.WaypointIndex < GM.I.Waypoints.Length - 1)
                            if (!waypointReached)                    
                                MoveAndRotateCreep(creep);                                                     
                            else
                                creep.WaypointIndex++;                    
                        else
                            DestroyCreep(creep);
                    }
                }
            }

            #region  Helper functions

            void MoveAndRotateCreep(CreepSystem creep)
            {           
                var creepTransform = creep.Prefab.transform;
                creepTransform.Translate(Vector3.forward * Time.deltaTime * creep.Stats.MoveSpeed, Space.Self);

                var clampPos = 
                    new Vector3(
                        creepTransform.position.x, 
                        creepTransform.lossyScale.y, 
                        creepTransform.position.z);
                        
                creepTransform.position = clampPos;

                RotateCreep();

                void RotateCreep()
                {            
                    var lookRotation = 
                        Quaternion.LookRotation(GM.I.Waypoints[creep.WaypointIndex].transform.position - creepTransform.position);
                    var rotation =
                        Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);

                    rotation.z = 0;
                    rotation.x = 0;

                    creepTransform.localRotation = rotation;
                }
            }
            
            #endregion
        }

        public static void DestroyCreep(CreepSystem creep)
        {                     
            if (creep != null)
            {
                GM.I.Creeps.Remove(creep);
                Object.Destroy(creep.Stats);
                Object.Destroy(creep.Prefab);
            }
        }    
	}
}
