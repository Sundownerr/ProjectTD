
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
                CachedTransform = transform;

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

