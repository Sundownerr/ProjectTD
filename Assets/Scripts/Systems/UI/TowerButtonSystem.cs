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
            if (GM.PLAYERSTATE != GM.PLACING_TOWER && GM.PLAYERSTATE != GM.PREPARE_PLACING_TOWER)
            {
                GM.Instance.BuildUISystem.IsChoosedNewTower = true;
                GM.Instance.PlayerInputSystem.NewTowerData = TowerData;

                Count--;

                Destroy(gameObject);
            }           
        }
    }
}

