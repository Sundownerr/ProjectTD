using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;

namespace Game.Tower
{
    public class TowerRangeSystem : ExtendedMonoBehaviour
    {
        public List<GameObject> CreepList;
        public List<Creep.CreepSystem> CreepSystemList;

        private Renderer rend;
        private Color transparent, notTransparent;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            CreepList = new List<GameObject>();
            CreepSystemList = new List<Creep.CreepSystem>();
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
                    CreepSystemList.Add(other.gameObject.GetComponent<Creep.CreepSystem>());
                    CreepList.Add(other.gameObject);                    
                }
            }                     
        }    

        private void OnTriggerExit(Collider other)
        {
            if (CreepList.Count > 0)
            {
                CreepSystemList.Remove(other.gameObject.GetComponent<Creep.CreepSystem>());
                CreepList.Remove(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            for (int i = 0; i < CreepList.Count; i++)
            {
                if (CreepList[i] == null)
                {
                    CreepList.RemoveAt(i);
                    CreepSystemList.RemoveAt(i);
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
