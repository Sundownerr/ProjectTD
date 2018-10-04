using System.Collections;
using System;
using UnityEngine;
using Game.System;
using Game.Data.Entity.Creep;
#pragma warning disable CS1591 
namespace Game.Creep
{

    public class CreepSystem : ExtendedMonoBehaviour
    {
        public bool ReachedLastWaypoint;
        public CreepStats Stats;

        private Transform creepTransform;
        private bool waypointReached;
        private int waypointIndex;  
        
        private void Start()
        {
            GameManager.Instance.CreepList.Add(gameObject);
            transform.parent = GameManager.Instance.CreepParent;

            Stats = ScriptableObject.CreateInstance<CreepStats>();

            Stats.hp = 100f;
            Stats.entityName = "retard";
            Stats.armorIndex = 0;
            Stats.moveSpeed = 250f;            

            creepTransform = transform;
            creepTransform.position = GameManager.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);             
        }

        private void Update()
        {
            waypointReached = GameManager.CalcDistance(creepTransform.position, GameManager.Instance.WaypointList[waypointIndex].transform.position) < 70;

            if (waypointIndex < GameManager.Instance.WaypointList.Length - 1)
            {
                if (!waypointReached)
                {
                    MoveCreep();
                    RotateCreep();
                }
                else
                {
                    waypointIndex++;
                }
            }
            else
            {
                RemoveCreep();
            }
        }

        private void MoveCreep()
        {
            creepTransform.Translate(Vector3.forward * Time.deltaTime * Stats.moveSpeed, Space.Self);
            creepTransform.position = new Vector3(creepTransform.position.x, creepTransform.lossyScale.y, creepTransform.position.z);
        }

        private void RotateCreep()
        {
            var lookRotation = Quaternion.LookRotation(GameManager.Instance.WaypointList[waypointIndex].transform.position - creepTransform.position);

            var rotation = Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);
            rotation.z = 0;
            rotation.x = 0;

            creepTransform.localRotation = rotation;
        }

        public void GetDamage(int damage)
        {
            Stats.hp -= damage;

            if (Stats.hp <= 0)
            {
                RemoveCreep();
            }         
        }

        private void RemoveCreep()
        {
            Destroy(gameObject);
            GameManager.Instance.CreepList.Remove(gameObject);
        }      
    }
}