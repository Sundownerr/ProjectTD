using System.Collections;
using System.Collections.Generic;
using Game.Cells;
using UnityEngine;

namespace Game.Systems
{
    public class GridSystem 
    {
        public bool IsGridBuilded { get => isGridBuilded; set => isGridBuilded = value; }
        public List<Cell> Cells { get => cells; set => cells = value; }

        private bool isGridBuilded;
        private Color blue, red, green;
        private List<Cell> cells;

        public GridSystem()
        {
            GM.I.GridSystem = this;
            cells = new List<Cell>();

            CreateGrid();
 
            red = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            green = new Color(0.1f, 0.6f, 0.1f, 0.9f);
            blue = new Color(0.1f, 0.1f, 0.3f, 0.7f);      

            #region  Helper functions

            void CreateGrid()
            {
                CreateMainCell();

                for (int i = 0; i < cells.Count; i++)        
                    if (!cells[i].IsExpanded)
                        CellExpandSystem.Expand(cells[i]);     

                IsGridBuilded = true;            
            }   

            void CreateMainCell()
            {          
                for (var i = 0; i < GM.I.CellAreas.Length; i++)
                {
                    var ray = new Ray(GM.I.CellAreas[i].transform.position, Vector3.up);
                    var layerMask = 1 << 15;

                    if (!Physics.Raycast(ray, 100, layerMask))
                    {
                        var spawnPos = GM.I.CellAreas[i].transform.position + 
                            new Vector3(0, GM.I.CellAreas[i].transform.localScale.y / 1.9f, 0);

                        Object.Instantiate(GM.I.CellPrefab, spawnPos, Quaternion.identity, GM.I.CellParent);                       
                    }
                }
            }

            #endregion
        }       

        public void UpdateSystem()
        {
            if (IsGridBuilded)
            {
                var lastCell = cells[cells.Count - 1];

                if (GM.PlayerState == State.PlacingTower)
                {
                    if (!lastCell.gameObject.activeSelf)
                        SetCellsActive(true);

                    SetCellsColors();
                }
                else if (lastCell.gameObject.activeSelf)
                    SetCellsActive(false);
            }

            #region  Helper functions

            void SetCellsActive(bool active)
            {
                for (int i = 0; i < cells.Count; i++)
                    cells[i].gameObject.SetActive(active);
            }

            void SetCellsColors()
            {
                for (int i = 0; i < cells.Count; i++)
                {
                    var cell = cells[i];
                    cell.CellRenderer.material.color = cell.IsBusy ? red : cell.IsChosen ? blue : green;              
                }
            }

            #endregion
        }
    }
}
