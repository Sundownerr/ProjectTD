using UnityEngine;
using Game.System;

namespace Game.TowerCells
{
    public class TowerCell : ExtendedMonoBehaviour
    {
        public bool IsBusy, IsChosen;

        private void Start()
        {
            GameManager.Instance.TowerCellList.Add(gameObject);
            transform.parent = GameManager.Instance.TowerCellParent;

            new TowerCellExpandSystem(gameObject, GameManager.Instance.TowerCellPrefab, GameManager.Instance.TowerCellAreaList);

            gameObject.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.3f, 0.4f);
        }     
    }
}
