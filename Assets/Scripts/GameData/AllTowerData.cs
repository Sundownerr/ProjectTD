using Game.Data.Entity.Tower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "All Tower Data", menuName = "Data/All Tower Data")]

    [Serializable]
    public class AllTowerData : ScriptableObject
    {     
        [SerializeField]
        public ElementList AllTowerList;

        private string[] elementNameList;


        private void Awake()
        {
            var towerData = Resources.Load("All Tower Data");

            if (towerData == null)
            {
                AllTowerList.ElementsList = new List<Element>();

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
                    AllTowerList.ElementsList.Add(new Element()
                    {
                        Name = elementNameList[i],

                        RarityList = new List<Rarity>
                    {
                        new Rarity()
                        {
                            Name = "Common",
                            TowerList = new List<TowerData>()
                        },

                        new Rarity()
                        {
                            Name = "Uncommon",
                            TowerList = new List<TowerData>()
                        },

                         new Rarity()
                        {
                            Name = "Rare",
                            TowerList = new List<TowerData>()
                        },

                        new Rarity()
                        {
                            Name = "Unique",
                            TowerList = new List<TowerData>()
                        }
                    }
                    }
                    );
                }
            }
        }
    }
}