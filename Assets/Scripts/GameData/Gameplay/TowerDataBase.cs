
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.System;
using Game.Tower.Data;
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

            var towerData = Resources.Load("TowerDataBase");
            
            if(towerData is TowerDataBase data)
            {
                AllTowerList = data.GetAllTowerList();
            }
            else 
            {
                AllTowerList = new ElementList();                
                AllTowerList.ElementsList = new List<Element>();          
               
                var elementNameList = Enum.GetNames(typeof(ElementType));              
                         
                for (int i = 0; i < 7; i++)
                    AllTowerList.ElementsList.Add(new Element(elementNameList[i]));
            }                            
        }

        public ElementList GetAllTowerList() => AllTowerList;              
    }
}