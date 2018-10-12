using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.TowerCells
{

    public class Cell : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public bool IsBusy, IsChosen;

        [HideInInspector]
        public Renderer CellRenderer;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            GameManager.Instance.CellList.Add(gameObject);
            GameManager.Instance.CellStateList.Add(this);
            transform.parent = GameManager.Instance.CellParent;

            CellRenderer = GetComponent<Renderer>();
            CellRenderer.material.color = new Color(0, 0, 0, 0);        

            new CellExpandSystem(gameObject, GameManager.Instance.CellPrefab, GameManager.Instance.TowerCellAreaList);
        }
    }
}
