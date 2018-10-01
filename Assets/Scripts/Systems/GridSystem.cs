using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#pragma warning disable CS1591 
namespace Game.System
{
    public class GridSystem : MonoBehaviour
    {
        public bool IsGridBuilded;

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
                var layerMask = 1 << 9;
                layerMask = ~layerMask;

                if (!Physics.Raycast(ray, 100, layerMask))
                {
                    var spawnPos = GameManager.Instance.TowerCellAreaList[i].transform.position + new Vector3(0, GameManager.Instance.TowerCellAreaList[i].transform.localScale.y / 1.9f, 0);
                    Instantiate(GameManager.Instance.CellPrefab, spawnPos, Quaternion.Euler(0, 0, 0));
              
                }
            }
        }

        private void CellsSetActive(bool active)
        {
            for (int i = 0; i < GameManager.Instance.CellList.Count; i++)
            {
                GameManager.Instance.CellList[i].SetActive(active);
            }
        }

        private void Start()
        {
            StartCoroutine(BuildTimer());
        }

        private void Update()
        {
            if (IsGridBuilded)
            {
                if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    if (!GameManager.Instance.CellList[GameManager.Instance.CellList.Count - 1].activeSelf)
                    {
                        CellsSetActive(true);
                    }
                }

                if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
                {
                    if (GameManager.Instance.CellList[GameManager.Instance.CellList.Count - 1].activeSelf)
                    {
                        CellsSetActive(false);
                    }
                }
            }
        }
    }
}
