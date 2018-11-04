using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Tower.Data.Stats;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "TowerDataBase", menuName = "Data/Tower Data Base")]

    [Serializable]
    public class TowerDataBase : ScriptableObject
    {     
        [SerializeField]
        public ElementList AllTowerList;

        private void Awake()
        {    
            if(AllTowerList == null) 
            {
                AllTowerList = new ElementList();                
                AllTowerList.ElementsList = new List<Element>();          
               
                var elementNameList = Enum.GetNames(typeof(ElementType));              
                         
                for (int i = 0; i < elementNameList.Length; i++)
                    AllTowerList.ElementsList.Add(new Element(elementNameList[i]));
            }                            
        }     

        private void OnValidate()
        {
            for (int i = 0; i < AllTowerList.ElementsList.Count; i++)       
                for (int j = 0; j < AllTowerList.ElementsList[i].RarityList.Count; j++)          
                    for (int k = 0; k < AllTowerList.ElementsList[i].RarityList[j].TowerList.Count; k++)
                        if(AllTowerList.ElementsList[i].RarityList[j].TowerList[k] == null)
                            AllTowerList.ElementsList[i].RarityList[j].TowerList.RemoveAt(k);                          
        }      
    }
}