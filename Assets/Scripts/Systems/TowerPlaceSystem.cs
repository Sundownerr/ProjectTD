using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.TowerCells;

#pragma warning disable CS1591 
namespace Game.System
{
    public class TowerPlaceSystem : ExtendedMonoBehaviour
    {
        public Vector3 GhostedTowerPos;
        public Color GhostedTowerColor;
        public GameObject NewBusyCell;
        public LayerMask LayerMask;
        
        private bool isCanBuild, isTowerCreated;
        private Color transparentRed, transparentGreen;
        private List<Cell> towerCellStateList;       
        private RaycastHit hit;
        private Camera mainCam;
       
        private void Start()
        {
            towerCellStateList = new List<Cell>();
            mainCam = Camera.main;

            StartCoroutine(GetTowerCellData());

            transparentRed = Color.red - new Color(0, 0, 0, 0.8f);
            transparentGreen = Color.green - new Color(0, 0, 0, 0.8f);
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

                MoveGhostedTower();
            }
        }

        private IEnumerator Refresh()
        {
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
            yield return new WaitForFixedUpdate();
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
        }

        private IEnumerator GetTowerCellData()
        {
            while (!GameManager.Instance.GridSystem.IsGridBuilded)
            {
                yield return new WaitForSeconds(0.05f);
            }

            for (int i = 0; i < GameManager.Instance.CellList.Count; i++)
            {
                towerCellStateList.Add(GameManager.Instance.CellList[i].GetComponent<Cell>());
            }

            isCanBuild = true;
        }

        private void CreateGhostedTower()
        {
            GameManager.Instance.TowerList.Add(Instantiate(GameManager.Instance.TowerPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 0f), GameManager.Instance.TowerParent));
            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
        }

        private void SetTowerColorAndPosition(Vector3 pos, Color color)
        {
            GhostedTowerPos = pos;
            GhostedTowerColor = color;
        }

        private void MoveGhostedTower()
        {          
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var towerCellList = GameManager.Instance.CellList;

            if (Physics.Raycast(ray, out hit, 5000, LayerMask))
            {
                SetTowerColorAndPosition(hit.point, transparentRed);                        

                for (int i = 0; i < towerCellList.Count; i++)
                {                   
                    var isHitTowerCell = hit.transform.gameObject == towerCellList[i];

                    if (isHitTowerCell && !towerCellStateList[i].IsBusy)
                    {
                        SetTowerColorAndPosition(towerCellList[i].transform.position, transparentGreen);

                        towerCellStateList[i].IsChosen = true;

                        if (Input.GetMouseButtonDown(0))
                        {                       
                            towerCellStateList[i].IsBusy = true;
                            NewBusyCell = towerCellList[i];

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
            var towerList = GameManager.Instance.TowerList;
            var lastTowerIndex = towerList.Count - 1;

            Destroy(towerList[lastTowerIndex]);
            towerList.RemoveAt(lastTowerIndex);

            isTowerCreated = false;

            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;          
        }      
    }
}