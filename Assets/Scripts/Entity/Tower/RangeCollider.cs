using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tower
{

    public class RangeCollider : MonoBehaviour
    {

        public bool IsCreepInRange;
        public List<GameObject> CreepInRangeList;

        private void Start()
        {
            CreepInRangeList = new List<GameObject>();
        }

        private void OnTriggerEnter(Collider other)
        {
            CreepInRangeList.Add(other.gameObject);
            IsCreepInRange = true;
        }

        private void OnTriggerStay(Collider other)
        {

        }

        private void OnTriggerExit(Collider other)
        {
            if (CreepInRangeList.Count > 0)
            {
                CreepInRangeList.RemoveAt(0);
            }

            if (CreepInRangeList.Count == 0)
            {
                IsCreepInRange = false;
            }
        }

    }
}
