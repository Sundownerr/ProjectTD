using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#pragma warning disable CS1591 
namespace Game.System
{
    public class GridSystem : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public bool IsGridBuilded;

        public LayerMask LayerMask;

        private Color blue, red, green;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            StartCoroutine(BuildTimer());
            GM.Instance.GridSystem = this;

            red = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            green = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blue = new Color(0.1f, 0.1f, 0.3f, 0.4f);
        }

        private void Update()
        {
            if (IsGridBuilded)
            {
                var lastCell = GM.Instance.CellList[GM.Instance.CellList.Count - 1];

                if (GM.PLAYERSTATE == GM.PLAYERSTATE_PLACINGTOWER)
                {
                    if (!lastCell.activeSelf)
                    {
                        SetCellsActive(true);
                    }

                    SetCellsColors();
                }

                if (GM.PLAYERSTATE != GM.PLAYERSTATE_PLACINGTOWER)
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
            CreateGrid(GM.Instance.TowerCellAreaList.Length);

            yield return new WaitForSeconds(1f);

            IsGridBuilded = true;
        }

        private void CreateGrid(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var ray = new Ray(GM.Instance.TowerCellAreaList[i].transform.position, Vector3.up);

                if (!Physics.Raycast(ray, 100, LayerMask))
                {
                    var spawnPos = GM.Instance.TowerCellAreaList[i].transform.position + new Vector3(0, GM.Instance.TowerCellAreaList[i].transform.localScale.y / 1.9f, 0);
                    Instantiate(GM.Instance.CellPrefab, spawnPos, Quaternion.identity);            
                }
            }
        }

        private void SetCellsActive(bool active)
        {
            for (int i = 0; i < GM.Instance.CellList.Count; i++)
            {
                GM.Instance.CellList[i].SetActive(active);
            }
        }

        private void SetCellsColors()
        {
            for (int i = 0; i < GM.Instance.CellStateList.Count; i++)
            {
                var cell = GM.Instance.CellStateList[i];

                if (cell.IsBusy)
                {
                    cell.CellRenderer.material.color = red;
                }
                else if (cell.IsChosen)
                {
                    cell.CellRenderer.material.color = green;
                }
                else
                {
                    cell.CellRenderer.material.color = blue;
                }
            }
        }
    }
}
