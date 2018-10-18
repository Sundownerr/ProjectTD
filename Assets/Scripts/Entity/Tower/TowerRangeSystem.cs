using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Tower
{
    public class TowerRangeSystem : ExtendedMonoBehaviour
    {
        public List<GameObject> CreepInRangeList;
        public List<Creep.CreepSystem> CreepInRangeSystemList;

        private Renderer rend;
        private Color transparent, notTransparent;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            CreepInRangeList = new List<GameObject>();
            CreepInRangeSystemList = new List<Creep.CreepSystem>();
            transform.position += new Vector3(0, -5, 0);
            rend = GetComponent<Renderer>();

            transparent = new Color(0f, 0f, 0f, 0f);
            notTransparent = new Color(0, 0.5f, 0, 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < GM.Instance.CreepList.Count; i++)
            {
                if (other.gameObject == GM.Instance.CreepList[i])
                {
                    CreepInRangeSystemList.Add(other.gameObject.GetComponent<Creep.CreepSystem>());
                    CreepInRangeList.Add(other.gameObject);                    
                }
            }                     
        }    

        private void OnTriggerExit(Collider other)
        {
            if (CreepInRangeList.Count > 0)
            {
                CreepInRangeSystemList.Remove(other.gameObject.GetComponent<Creep.CreepSystem>());
                CreepInRangeList.Remove(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            for (int i = 0; i < CreepInRangeList.Count; i++)
            {
                if (CreepInRangeList[i] == null)
                {
                    CreepInRangeList.RemoveAt(i);
                    CreepInRangeSystemList.RemoveAt(i);
                }
            }
        }

        public void Show(bool show)
        {
            if (show)
            {
                if (rend.material.color != notTransparent)
                {
                    rend.material.color = notTransparent;
                }
            }
            else
            {
                if (rend.material.color != transparent)
                {
                    rend.material.color = transparent;
                }
            }
        }            
    }
}
