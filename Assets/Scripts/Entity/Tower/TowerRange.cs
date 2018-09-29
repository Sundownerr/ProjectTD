using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS1591 
namespace Game.Tower
{

    public class TowerRange : ExtendedMonoBehaviour
    {
        public bool IsCreepInRange;
        public List<GameObject> CreepInRangeList;

        private Renderer rend;

        private IEnumerator DeleteMissing()
        {
            while (CreepInRangeList.Count > 0)
            {
                for (int i = 0; i < CreepInRangeList.Count; i++)
                {
                    if (CreepInRangeList[i] == null)
                    {
                        CreepInRangeList.RemoveAt(i);
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public void Show(bool show)
        {
            if (show)
            {
                if (rend.material.color != new Color(0, 0.5f, 0, 0.2f))
                {
                    rend.material.color = new Color(0, 0.5f, 0, 0.2f);
                }
            }
            else
            {
                if (rend.material.color != new Color(0f, 0f, 0f, 0f))
                {
                    rend.material.color = new Color(0f, 0f, 0f, 0f);
                }
            }
        }

        private void Start()
        {
            CreepInRangeList = new List<GameObject>();
            transform.position += new Vector3(0, -5, 0);
            rend = GetComponent<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            CreepInRangeList.Add(other.gameObject);
            IsCreepInRange = true;

            if(CreepInRangeList.Count == 1)
            {   
                StartCoroutine(DeleteMissing());
            }
        }
  
        private void OnTriggerExit(Collider other)
        {
            if (CreepInRangeList.Count > 0)
            {               
                CreepInRangeList.RemoveAt(0);
            }

            if (CreepInRangeList.Count == 0)
            {
                StopAllCoroutines();
                IsCreepInRange = false;
            }
        }
    }
}
