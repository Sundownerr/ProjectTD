using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    [System.Serializable]
    public class ObjectPool
    {

    public GameObject poolObject;
    public Transform parent;
    public uint poolLenght = 5;

    protected List<GameObject> poolList = new List<GameObject>();

    public void Initialize()
    {
        if (poolObject == null)
        {
            Debug.LogError("ObjectPooler missing prefab");
            return;
        }
        for (int i = 0; i < poolLenght; ++i)
        {
            CreateObject();
        }
    }

    public GameObject GetObject()
    {
        for (int i = 0; i < poolList.Count; ++i)
        {
            if (!poolList[i].activeInHierarchy)
            {
                return poolList[i];
            }
        }

        if (poolObject == null)
        {
            return null;
        }

        CreateObject();

        return poolList[poolList.Count - 1];
    }

    public void DestroyPool()
    {
        for (int i = poolList.Count - 1; i >= 0; --i)
        {
            Object.Destroy(poolList[i]);
        }
        poolList.Clear();
    }

    protected void CreateObject()
    {
        GameObject go = Object.Instantiate(poolObject) as GameObject;

        go.SetActive(false);

        if (parent != null)
        {
            go.transform.parent = parent;
            go.transform.position = go.transform.parent.position;
        }
        poolList.Add(go);
    }

}