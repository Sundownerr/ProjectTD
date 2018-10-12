using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#pragma warning disable CS1591 
namespace Game.System
{
    public class GridSystem : MonoBehaviour
    {
        [HideInInspector]
        public bool IsGridBuilded;

        public LayerMask LayerMask;

        private Color blueColor, redColor, greenColor;

        private void Start()
        {
            StartCoroutine(BuildTimer());
            GameManager.Instance.GridSystem = this;

            redColor = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            greenColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blueColor = new Color(0.1f, 0.1f, 0.3f, 0.4f);
        }

        private void Update()
        {
            if (IsGridBuilded)
            {
                var lastCell = GameManager.Instance.CellList[GameManager.Instance.CellList.Count - 1];

                if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    if (!lastCell.activeSelf)
                    {
                        SetCellsActive(true);
                    }

                    SetCellsColors();
                }

                if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    if (lastCell.activeSelf)
                    {
                        SetCellsActive(false);
                    }             
                }
            }
        }

        private IEnumerator BuildTimer()
        {
            CreateGrid(GameManager.Instance.TowerCellAreaList.Length);

            yield return new WaitForSeconds(1f);

            IsGridBuilded = true;
        }

        private void CreateGrid(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var ray = new Ray(GameManager.Instance.TowerCellAreaList[i].transform.position, Vector3.up);

                if (!Physics.Raycast(ray, 100, LayerMask))
                {
                    var spawnPos = GameManager.Instance.TowerCellAreaList[i].transform.position + new Vector3(0, GameManager.Instance.TowerCellAreaList[i].transform.localScale.y / 1.9f, 0);
                    Instantiate(GameManager.Instance.CellPrefab, spawnPos, Quaternion.Euler(0, 0, 0));            
                }
            }
        }

        private void SetCellsActive(bool active)
        {
            for (int i = 0; i < GameManager.Instance.CellList.Count; i++)
            {
                GameManager.Instance.CellList[i].SetActive(active);
            }
        }

        private void SetCellsColors()
        {
            for (int i = 0; i < GameManager.Instance.CellStateList.Count; i++)
            {
                var cell = GameManager.Instance.CellStateList[i];

                if (cell.IsBusy)
                {
                    cell.CellRenderer.material.color = redColor;
                }
                else if (cell.IsChosen)
                {
                    cell.CellRenderer.material.color = greenColor;
                }
                else
                {
                    cell.CellRenderer.material.color = blueColor;
                }
            }
        }
    }
}
