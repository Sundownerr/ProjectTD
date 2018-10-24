using Game.Data.Entity.Tower;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Element 
{
    public List<Rarity> Rarities;
    public string Name;
}

[Serializable]
public class Rarity
{
    
    public List<TowerData> Towers;
    public string Name;
}
