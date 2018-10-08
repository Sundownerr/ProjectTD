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
        [HideInInspector]
        public bool ReachedLastWaypoint;

        public CreepStats Stats;

        [HideInInspector]
        public Renderer creepRenderer;

        private Transform creepTransform;
        private bool waypointReached;
        private int waypointIndex;  
        
        private void Start()
        {
            GameManager.Instance.CreepList.Add(gameObject);
            transform.parent = GameManager.Instance.CreepParent;

            creepRenderer = transform.GetChild(0).GetComponent<Renderer>();

            Stats = Instantiate(Stats);              

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
                Destroy(gameObject);
            }
        }

        private void MoveCreep()
        {
            creepTransform.Translate(Vector3.forward * Time.deltaTime * Stats.MoveSpeed, Space.Self);
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
            Stats.Health -= damage;

            if (Stats.Health <= 0)
            {
                
                Destroy(gameObject);
            }         
        }

        private void OnDestroy()
        {
            Destroy(Stats);
            GameManager.Instance.CreepList.Remove(gameObject);
        }      
    }
}