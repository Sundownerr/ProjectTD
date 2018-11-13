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
            GM.I.GridSystem = this;

            CreateGrid();
 
            red = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            green = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blue = new Color(0.1f, 0.1f, 0.3f, 0.4f);      

            void CreateGrid()
            {
                cellExpandSystem = new CellExpandSystem();

                CreateMainCell();

                for (int i = 0; i < GM.I.CellStateList.Count; i++)        
                    if(!GM.I.CellStateList[i].IsExpanded)
                        cellExpandSystem.Expand(GM.I.CellStateList[i]);     

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

                        Object.Instantiate(GM.I.CellPrefab, spawnPos, Quaternion.identity);                       
                    }
                }
            }
        }       

        public void Update()
        {
            if (IsGridBuilded)
            {
                var lastCell = GM.I.CellList[GM.I.CellList.Count - 1];

                if (GM.PlayerState == State.PlacingTower)
                {
                    if (!lastCell.activeSelf)
                        SetCellsActive(true);

                    SetCellsColors();
                }
                else if (lastCell.activeSelf)
                    SetCellsActive(false);
            }

            void SetCellsActive(bool active)
            {
                for (int i = 0; i < GM.I.CellList.Count; i++)
                    GM.I.CellList[i].SetActive(active);
            }

            void SetCellsColors()
            {
                for (int i = 0; i < GM.I.CellStateList.Count; i++)
                {
                    var cell = GM.I.CellStateList[i];
                    cell.CellRenderer.material.color = cell.IsBusy ? red : cell.IsChosen ? green : blue;              
                }
            }
        }
    }
}
