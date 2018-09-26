using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.TowerCells;


namespace Game.System
{
    public class TowerPlaceSystem : MonoBehaviour
    {
        public Color GhostedTowerColor;

        private List<TowerCell> towerCellStateList;
        private bool canBuild, isTowerCreated;
        private RaycastHit hit;

        private IEnumerator Refresh()
        {
            GameManager.Instance.UISystem.IsBuildModeActive = false;

            yield return new WaitForSeconds(0.05f);

            GameManager.Instance.UISystem.IsBuildModeActive = true;
        }

        private IEnumerator GetTowerCellData()
        {
            while (!GameManager.Instance.GridSystem.IsGridBuilded)
            {
                yield return new WaitForSeconds(0.05f);
            }

            for (int i = 0; i < GameManager.Instance.TowerCellList.Count; i++)
            {
                towerCellStateList.Add(GameManager.Instance.TowerCellList[i].GetComponent<TowerCell>());
            }

            canBuild = true;
        }

        private void CreateGhostedTower()
        {
            GameManager.Instance.TowerList.Add(Instantiate(GameManager.Instance.TowerPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f), GameManager.Instance.TowerParent));
        }

        private void PlaceGhostedTower()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit))
            {
                GhostedTowerColor = Color.red - new Color(0, 0, 0, 0.8f);
                GameManager.Instance.TowerList[GameManager.Instance.TowerList.Count - 1].transform.position = hit.point;

                for (int i = 0; i < GameManager.Instance.TowerCellList.Count; i++)
                {
                    if (hit.transform.gameObject == GameManager.Instance.TowerCellList[i] && !towerCellStateList[i].IsBusy)
                    {
                        GameManager.Instance.TowerList[GameManager.Instance.TowerList.Count - 1].transform.position = GameManager.Instance.TowerCellList[i].transform.position;
                        GhostedTowerColor = Color.green - new Color(0, 0, 0, 0.8f);
                        towerCellStateList[i].IsChosen = true;

                        if (Input.GetMouseButtonDown(0))
                        {
                            towerCellStateList[i].IsBusy = true;

                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                isTowerCreated = false;

                                StartCoroutine(Refresh());
                            }
                            else
                            {
                                GameManager.Instance.UISystem.IsBuildModeActive = false;
                            }
                        }
                    }
                    else
                    {
                        towerCellStateList[i].IsChosen = false;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    DeleteTower();
                }
            }
        }

        private void DeleteTower()
        {
            var lastTowerIndex = GameManager.Instance.TowerList.Count - 1;

            Destroy(GameManager.Instance.TowerList[lastTowerIndex]);
            GameManager.Instance.TowerList.RemoveAt(lastTowerIndex);

            GameManager.Instance.UISystem.IsBuildModeActive = false;
        }

        private void Start()
        {
            towerCellStateList = new List<TowerCell>();

            StartCoroutine(GetTowerCellData());
        }

        private void Update()
        {
            if (GameManager.Instance.UISystem.IsBuildModeActive && canBuild)
            {
                if (!isTowerCreated)
                {
                    CreateGhostedTower();

                    isTowerCreated = true;
                }

                PlaceGhostedTower();
            }
            else
            {
                isTowerCreated = false;
            }
        }
    }
}