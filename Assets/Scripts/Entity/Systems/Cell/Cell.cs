using Game.Systems;
using UnityEngine;

namespace Game.Cells
{

    public class Cell : ExtendedMonoBehaviour
    {
        
        public bool IsBusy { get => isBusy; set => isBusy = value; }
        public bool IsChosen { get => isChosen; set => isChosen = value; }
        public Renderer CellRenderer { get => cellRenderer; set => cellRenderer = value; }
        public bool IsExpanded { get => isExpanded; set => isExpanded = value; }
        
        private bool isChosen, isBusy, isExpanded;
        private Renderer cellRenderer;

        protected override void Awake()
        {
            base.Awake();
        
            GM.I.GridSystem.CellList.Add(this);

            cellRenderer = GetComponent<Renderer>();
            cellRenderer.material.color = new Color(0, 0, 0, 0);      
        }
    }
}