using Game.Data.Entity.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "All Tower Data", menuName = "All Tower Data")]

    [Serializable]
    public class AllTowerData : ScriptableObject
    {     
    
        public List<Element> AllTowerList;

        private string[] elementNameList;

        private void Awake()
        {
            AllTowerList = new List<Element>();

            elementNameList = new string[]
            {
                "Astral",
                "Darkness",
                "Ice",
                "Iron",
                "Storm",
                "Nature",
                "Fire"
            };

            for (int i = 0; i < 7; i++)
            {
                AllTowerList.Add(new Element()
                {
                    Name = elementNameList[i],

                    Rarities = new List<Rarity>
                    {
                        new Rarity()
                        {
                            Name = "Common",
                            Towers = new List<TowerData>()
                        },

                        new Rarity()
                        {
                            Name = "Uncommon",
                            Towers = new List<TowerData>()
                        },

                         new Rarity()
                        {
                            Name = "Rare",
                            Towers = new List<TowerData>()
                        },

                        new Rarity()
                        {
                            Name = "Unique",
                            Towers = new List<TowerData>()
                        }
                    }
                }
                );                  
            }               
        }
    }
}