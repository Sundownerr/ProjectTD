using UnityEngine.UI;
using Game.Tower.Data;
using UnityEngine;
using TMPro;
namespace Game.Systems
{
    public class TowerButtonSystem : ExtendedMonoBehaviour
    {
        public int Count { get => count; set => count = value; }
        public TowerData TowerData { get => towerData; set => towerData = value; }

        private TowerData towerData;
        private int count;
        private TextMeshProUGUI towerCountText;

        protected override void Awake()
        {
            base.Awake();

            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ClickTowerButton);
            towerCountText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        private void ClickTowerButton()
        {          
            if (GM.PlayerState != State.PlacingTower && GM.PlayerState != State.PreparePlacingTower)
            {
                GM.Instance.BuildUISystem.IsChoosedNewTower = true;
                GM.Instance.PlayerInputSystem.NewTowerData = TowerData;
                Count--;
                towerCountText.text = Count.ToString();
                
                if(Count <= 1)
                {
                    GM.Instance.BuildUISystem.RemoveTowerButton(this);
                    Destroy(gameObject);
                }              
            }           
        }
    }
}

