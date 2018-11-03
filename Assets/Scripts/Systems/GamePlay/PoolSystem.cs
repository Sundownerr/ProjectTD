using UnityEngine;
using System.Collections.Generic;

namespace Game.Systems
{
    public class ObjectPool
    {
        public GameObject PoolObject { get => poolObject; set => poolObject = value; }
        public Transform Parent { get => parent; set => parent = value; }
        public uint PoolLenght { get => poolLenght; set => poolLenght = value; }

        private GameObject poolObject;
        private Transform parent;
        private uint poolLenght = 1;
        private List<GameObject> poolList = new List<GameObject>();
     
        public void Initialize()
        {
            if (PoolObject == null)
            {
                Debug.LogError("ObjectPooler missing prefab");
                return;
            }

            for (int i = 0; i < PoolLenght; i++)
                CreateObject(Parent);
        }

        public GameObject GetObject()
        {
            for (int i = 0; i < poolList.Count; i++)
                if (!poolList[i].activeSelf)
                    return poolList[i];

            if (PoolObject == null)
                return null;

            CreateObject(Parent);

            return poolList[poolList.Count - 1];
        }

        public void DestroyPool()
        {
            for (int i = poolList.Count - 1; i > 0; i--)
                Object.Destroy(poolList[i]);

            poolList.Clear();
        }

        protected void CreateObject(Transform parent)
        {
            poolList.Add(Object.Instantiate(PoolObject, parent));
            poolList[poolList.Count - 1].SetActive(false);
        }
    }
}