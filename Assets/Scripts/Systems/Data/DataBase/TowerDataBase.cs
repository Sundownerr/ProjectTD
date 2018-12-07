using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Tower.Data.Stats;

namespace Game.Data
{   
    [CreateAssetMenu(fileName = "TowerDB", menuName = "Data/Data Base/Tower DataBase")]
    [Serializable]
    public class TowerDataBase : ScriptableObject, IData
    {     
        [SerializeField]
        public ElementList AllTowers;     

        private void Awake()
        {    
            if (AllTowers == null) 
            {
                AllTowers = new ElementList();                
                AllTowers.Elements = new List<Element>();          
               
                var elementNames = Enum.GetNames(typeof(ElementType));              
                         
                for (int i = 0; i < elementNames.Length; i++)
                    AllTowers.Elements.Add(new Element(elementNames[i]));
            }                            
        }     
    }
}