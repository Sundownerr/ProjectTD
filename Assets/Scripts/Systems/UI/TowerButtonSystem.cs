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
        public TextMeshProUGUI TowerCountText { get => towerCountText; set => towerCountText = value; }

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
                if (GM.I.ResourceSystem.CheckHaveResources(towerData.TowerLimit, towerData.GoldCost, towerData.MagicCrystalReq))
                {
                    GM.I.PlayerInputSystem.NewTowerData = towerData;              
                    GM.I.BuildUISystem.BuildNewTower();           
                    count--;              
                
                    if(count >= 1)
                        towerCountText.text = count.ToString();     
                    else
                    {
                        GM.I.BuildUISystem.RemoveTowerButton(this);
                        Destroy(gameObject);
                    }
                }                                 
        }
    }
}

