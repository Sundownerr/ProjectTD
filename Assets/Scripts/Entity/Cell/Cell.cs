using UnityEngine;
using Game.Systems;
#pragma warning disable CS1591 
namespace Game.Cells
{

    public class Cell : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public bool IsBusy, IsChosen;

        [HideInInspector]
        public Renderer CellRenderer;

        protected override void Awake()
        {
            base.Awake();

            GM.Instance.CellList.Add(gameObject);
            GM.Instance.CellStateList.Add(this);
            transform.parent = GM.Instance.CellParent;

            CellRenderer = GetComponent<Renderer>();
            CellRenderer.material.color = new Color(0, 0, 0, 0);        

            new CellExpandSystem(gameObject, GM.Instance.CellPrefab, GM.Instance.CellAreaList);
        }
    }
}
