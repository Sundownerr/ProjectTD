using System.Collections;
using Game.Cells;
using UnityEngine;

namespace Game.Systems
{
    public class GridSystem 
    {
        public bool IsGridBuilded { get => isGridBuilded; set => isGridBuilded = value; }

        private bool isGridBuilded;
        private Color blue, red, green;
        private CellExpandSystem cellExpandSystem;

        public GridSystem()
        {
            GM.Instance.GridSystem = this;

            CreateGrid();
 
            red = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            green = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blue = new Color(0.1f, 0.1f, 0.3f, 0.4f);         
        }       

        public void Update()
        {
            if (IsGridBuilded)
            {
                var lastCell = GM.Instance.CellList[GM.Instance.CellList.Count - 1];

                if (GM.PlayerState == State.PlacingTower)
                {
                    if (!lastCell.activeSelf)
                        SetCellsActive(true);

                    SetCellsColors();
                }
                else if (lastCell.activeSelf)
                    SetCellsActive(false);
            }
        }

        private void CreateGrid()
        {
            cellExpandSystem = new CellExpandSystem();

            CreateMainCell();

            for (int i = 0; i < GM.Instance.CellStateList.Count; i++)        
                if(!GM.Instance.CellStateList[i].IsExpanded)
                    cellExpandSystem.Expand(GM.Instance.CellStateList[i]);     

            IsGridBuilded = true;
        }

        private void CreateMainCell()
        {          
            for (var i = 0; i < GM.Instance.CellAreaList.Length; i++)
            {
                var ray = new Ray(GM.Instance.CellAreaList[i].transform.position, Vector3.up);
                var layerMask = 1 << 15;

                if (!Physics.Raycast(ray, 100, layerMask))
                {
                    var spawnPos = GM.Instance.CellAreaList[i].transform.position + 
                        new Vector3(0, GM.Instance.CellAreaList[i].transform.localScale.y / 1.9f, 0);

                    Object.Instantiate(GM.Instance.CellPrefab, spawnPos, Quaternion.identity);                       
                }
            }
        }

        private void SetCellsActive(bool active)
        {
            for (int i = 0; i < GM.Instance.CellList.Count; i++)
                GM.Instance.CellList[i].SetActive(active);
        }

        private void SetCellsColors()
        {
            for (int i = 0; i < GM.Instance.CellStateList.Count; i++)
            {
                var cell = GM.Instance.CellStateList[i];
                cell.CellRenderer.material.color = cell.IsBusy ? red : cell.IsChosen ? green : blue;              
            }
        }
    }
}
