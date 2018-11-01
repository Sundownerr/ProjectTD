﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.System;
using Game.Tower.Data;
using Game.Tower.Data.Stats;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "All Tower Data", menuName = "Data/All Tower Data")]

    [Serializable]
    public class AllTowerData : ScriptableObject
    {     
        [SerializeField]
        public ElementList AllTowerList;

        private void Awake()
        {
            var towerData = Resources.Load("All Tower Data");

            if (towerData == null)
            {
                AllTowerList.ElementsList = new List<Element>();          

                for (int i = 0; i < 7; i++)
                    AllTowerList.ElementsList.Add(
                        new Element()
                        {
                            Name = GM.Instance.ElementNameList[i],

                            RarityList = new List<Rarity>
                            {
                                new Rarity(new List<TowerData>(), "Common"),
                                new Rarity(new List<TowerData>(), "Uncommon"),
                                new Rarity(new List<TowerData>(), "Rare"),
                                new Rarity(new List<TowerData>(), "Unique")
                            }
                        }
                    );
            }
        }
    }
}