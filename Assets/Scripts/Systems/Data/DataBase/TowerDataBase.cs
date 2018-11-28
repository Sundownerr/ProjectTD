using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Tower.Data.Stats;

namespace Game.Data
{   
    [CreateAssetMenu(fileName = "TowerDB", menuName = "Data/Tower DataBase")]
    [Serializable]
    public class TowerDataBase : ScriptableObject, IData
    {     
        [SerializeField]
        public ElementList AllTowerList;     

        private void Awake()
        {    
            if (AllTowerList == null) 
            {
                AllTowerList = new ElementList();                
                AllTowerList.ElementsList = new List<Element>();          
               
                var elementNameList = Enum.GetNames(typeof(ElementType));              
                         
                for (int i = 0; i < elementNameList.Length; i++)
                    AllTowerList.ElementsList.Add(new Element(elementNameList[i]));
            }                            
        }     
    }
}