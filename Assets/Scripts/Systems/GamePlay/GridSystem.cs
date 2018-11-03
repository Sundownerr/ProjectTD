using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class GridSystem 
    {
        public bool IsGridBuilded { get => isGridBuilded; set => isGridBuilded = value; }

        private bool isGridBuilded;
        private Color blue, red, green;

        public GridSystem()
        {
            GM.Instance.StartCoroutine(BuildTimer());
            GM.Instance.GridSystem = this;

            red = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            green = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blue = new Color(0.1f, 0.1f, 0.3f, 0.4f);
        }       

        public void Update()
        {
            if (IsGridBuilded)
            {
                var lastCell = GM.Instance.CellList[GM.Instance.CellList.Count - 1];

                if (GM.PlayerState == GM.State.PlacingTower)
                {
                    if (!lastCell.activeSelf)
                        SetCellsActive(true);

                    SetCellsColors();
                }
                else if (lastCell.activeSelf)
                    SetCellsActive(false);
            }
        }

        private IEnumerator BuildTimer()
        {
            CreateGrid(GM.Instance.CellAreaList.Length);
            
            yield return new WaitForSeconds(1f);

            IsGridBuilded = true;
        }

        private void CreateGrid(int count)
        {
            for (var i = 0; i < count; i++)
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
