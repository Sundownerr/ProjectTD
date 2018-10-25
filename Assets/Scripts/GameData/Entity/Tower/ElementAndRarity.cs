using Game.Data.Entity.Tower;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Element 
{
    [SerializeField]
    public List<Rarity> RarityList;

    [SerializeField]
    public string Name;
}

[Serializable]
public class Rarity
{
    [SerializeField]
    public List<TowerData> TowerList;
    
    [SerializeField]
    public string Name;
}

[Serializable]
public class ElementList
{
    [SerializeField]
    public List<Element> ElementsList;
}