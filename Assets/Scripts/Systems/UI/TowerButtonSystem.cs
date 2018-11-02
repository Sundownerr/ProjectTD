using UnityEngine.UI;
using Game.Tower.Data;

namespace Game.System
{
    public class TowerButtonSystem : ExtendedMonoBehaviour
    {
        public TowerData TowerData;
        public int Count;

        protected override void Awake()
        {
            base.Awake();

            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ClickTowerButton);
        }

        private void ClickTowerButton()
        {          
            if (GM.PlayerState != GM.State.PlacingTower && GM.PlayerState != GM.State.PreparePlacingTower)
            {
                GM.Instance.BuildUISystem.IsChoosedNewTower = true;
                GM.Instance.PlayerInputSystem.NewTowerData = TowerData;

                Count--;

                Destroy(gameObject);
            }           
        }
    }
}

