using System.Collections;
using System.Collections.Generic;
using Game.Cells;
using UnityEngine;

namespace Game.Systems
{
    public class GridSystem 
    {
        public bool IsGridBuilded { get => isGridBuilded; set => isGridBuilded = value; }
        public List<Cell> CellList { get => cellList; set => cellList = value; }

        private bool isGridBuilded;
        private Color blue, red, green;
        private CellExpandSystem cellExpandSystem;
        private List<Cell> cellList;

        public GridSystem()
        {
            GM.I.GridSystem = this;
            cellList = new List<Cell>();

            CreateGrid();
 
            red = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            green = new Color(0.1f, 0.6f, 0.1f, 0.9f);
            blue = new Color(0.1f, 0.1f, 0.3f, 0.7f);      

            void CreateGrid()
            {
                cellExpandSystem = new CellExpandSystem();

                CreateMainCell();

                for (int i = 0; i < cellList.Count; i++)        
                    if (!cellList[i].IsExpanded)
                        cellExpandSystem.Expand(cellList[i]);     

                IsGridBuilded = true;            
            }   

            void CreateMainCell()
            {          
                for (var i = 0; i < GM.I.CellAreaList.Length; i++)
                {
                    var ray = new Ray(GM.I.CellAreaList[i].transform.position, Vector3.up);
                    var layerMask = 1 << 15;

                    if (!Physics.Raycast(ray, 100, layerMask))
                    {
                        var spawnPos = GM.I.CellAreaList[i].transform.position + 
                            new Vector3(0, GM.I.CellAreaList[i].transform.localScale.y / 1.9f, 0);

                        Object.Instantiate(GM.I.CellPrefab, spawnPos, Quaternion.identity, GM.I.CellParent);                       
                    }
                }
            }
        }       

        public void UpdateSystem()
        {
            if (IsGridBuilded)
            {
                var lastCell = cellList[cellList.Count - 1];

                if (GM.PlayerState == State.PlacingTower)
                {
                    if (!lastCell.gameObject.activeSelf)
                        SetCellsActive(true);

                    SetCellsColors();
                }
                else if (lastCell.gameObject.activeSelf)
                    SetCellsActive(false);
            }

            void SetCellsActive(bool active)
            {
                for (int i = 0; i < cellList.Count; i++)
                    cellList[i].gameObject.SetActive(active);
            }

            void SetCellsColors()
            {
                for (int i = 0; i < cellList.Count; i++)
                {
                    var cell = cellList[i];
                    cell.CellRenderer.material.color = cell.IsBusy ? red : cell.IsChosen ? blue : green;              
                }
            }
        }
    }
}
