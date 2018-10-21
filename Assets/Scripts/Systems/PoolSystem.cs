﻿using UnityEngine;
using System.Collections.Generic;

    public class ObjectPool
    {
        public GameObject poolObject;
        public Transform parent;
        public uint poolLenght = 1;

        private List<GameObject> poolList = new List<GameObject>();

        public void Initialize()
        {
            if (poolObject == null)
            {
                Debug.LogError("ObjectPooler missing prefab");
                return;
            }

            for (int i = 0; i < poolLenght; i++)
            {
                CreateObject(parent);
            }
        }

        public GameObject GetObject()
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (!poolList[i].activeSelf)
                {
                    return poolList[i];
                }
            }

            if (poolObject == null)
            {
                return null;
            }

            CreateObject(parent);

            return poolList[poolList.Count - 1];
        }

        public void DestroyPool()
        {
            for (int i = poolList.Count - 1; i > 0; i--)
            {
                Object.Destroy(poolList[i]);
            }

            poolList.Clear();
        }

        protected void CreateObject(Transform parent)
        {
            poolList.Add(Object.Instantiate(poolObject, parent));

            poolList[poolList.Count - 1].SetActive(false);
        }

    }
