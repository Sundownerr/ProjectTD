using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.TowerCells;

#pragma warning disable CS1591 
namespace Game.System
{
    public class TowerPlaceSystem : ExtendedMonoBehaviour
    {
        public bool GhostedTowerVisible;
        public Color GhostedTowerColor;
        public GameObject NewBusyCell;
        public LayerMask LayerMask;
        public Vector3 GhostedTowerPos;

        private List<TowerCell> towerCellStateList;
        private bool isCanBuild, isTowerCreated, isOnCooldown;
        private RaycastHit hit;
        private Ray ray;

        private IEnumerator Refresh()
        {
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
            yield return new WaitForFixedUpdate();
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
            isOnCooldown = false;
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

            isCanBuild = true;
        }

        private void CreateGhostedTower()
        {
            GameManager.Instance.TowerList.Add(Instantiate(GameManager.Instance.TowerPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f), GameManager.Instance.TowerParent));
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
        }

        private void PlaceGhostedTower()
        {          
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 5000, LayerMask))
            {
                GhostedTowerColor = Color.red - new Color(0, 0, 0, 0.8f);
                GhostedTowerPos = hit.point;

                for (int i = 0; i < GameManager.Instance.TowerCellList.Count; i++)
                {
                    if (hit.transform.gameObject == GameManager.Instance.TowerCellList[i] && !towerCellStateList[i].IsBusy)
                    {
                        GhostedTowerPos = GameManager.Instance.TowerCellList[i].transform.position;

                        GhostedTowerColor = Color.green - new Color(0, 0, 0, 0.8f);
                        towerCellStateList[i].IsChosen = true;

                        if (Input.GetMouseButtonDown(0))
                        {                       
                            towerCellStateList[i].IsBusy = true;
                            NewBusyCell = GameManager.Instance.TowerCellList[i];

                            isTowerCreated = false;

                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                StartCoroutine(Refresh());
                            }
                            else
                            {
                                GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
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
           
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;

            isTowerCreated = false;
        }

        private void Start()
        {
            towerCellStateList = new List<TowerCell>();

            StartCoroutine(GetTowerCellData());
        }

        private void Update()
        {
            if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER && isCanBuild)
            {
                if (!isTowerCreated)
                {
                    CreateGhostedTower();

                    isTowerCreated = true;
                }

                PlaceGhostedTower();
            }
        }
    }
}