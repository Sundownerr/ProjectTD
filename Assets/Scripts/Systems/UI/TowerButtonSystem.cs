using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.System
{
    public class TowerButtonSystem : ExtendedMonoBehaviour
    {
        public Data.Entity.Tower.TowerData TowerData;
        public int Count;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ClickTowerButton);
        }

        private void ClickTowerButton()
        {          
            if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
            {
                GM.Instance.ChoosedTowerData = TowerData;

                GM.PLAYERSTATE = GM.PREPARE_PLACING_TOWER;
               
                for (int i = 0; i < GM.Instance.AvailableTowerList.Count; i++)
                {
                    if (GM.Instance.AvailableTowerList[i] == TowerData)
                    {
                        GM.Instance.AvailableTowerList.RemoveAt(i);
                        break;
                    }
                }

                GM.Instance.BuildUISystem.UpdateAvailableElement();
                GM.Instance.BuildUISystem.UpdateRarity(GM.Instance.ChoosedTowerData.ElementId);

                Count--;

                Destroy(gameObject);
            }           
        }
    }
}

