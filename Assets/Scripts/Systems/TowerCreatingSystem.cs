using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class TowerCreatingSystem : ExtendedMonoBehaviour
    {

        private List<int> LeveledElementList;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GM.Instance.TowerCreatingSystem = this;

            LeveledElementList = new List<int>();
        }
        
        public void CreateRandomTower()
        {
            LeveledElementList.Clear();

            var elementLevelList = GM.Instance.PlayerData.ElementLevelList;

            for (int i = 0; i < elementLevelList.Count; i++)
            {
                if (elementLevelList[i] > 0)
                {
                    LeveledElementList.Add(i);                 
                }
            }
        }
    }
}