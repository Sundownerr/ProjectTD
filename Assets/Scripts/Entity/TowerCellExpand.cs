﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.TowerCells
{

    public class TowerCellExpand
    {
        private bool FillSide( Vector3 spawnDirection, int checkMode, GameObject cell, GameObject cellPrefab, float rayDistance, int layerMask, float spacing)
        {
            var direction1 = new Vector3();
            var direction2 = new Vector3();

            if (checkMode == 1)
            {
                direction1 = Vector3.left;
                direction2 = Vector3.right;
            }
            else
            {
                direction1 = Vector3.forward;
                direction2 = Vector3.back;
            }

           
            {
                var sideRayHit =
                    Physics.Raycast(cell.transform.position + direction1 * cell.transform.lossyScale.x / 2, spawnDirection, rayDistance, layerMask) |
                    Physics.Raycast(cell.transform.position + direction2 * cell.transform.lossyScale.x / 2, spawnDirection, rayDistance, layerMask);

                if (!sideRayHit)
                {
                    Object.Instantiate(cellPrefab, cell.transform.position + spawnDirection * spacing, cell.transform.rotation).name = cell.transform.name;
                    return true;
                }
            }

            return false;
        }

        public TowerCellExpand(GameObject cell, GameObject cellPrefab, GameObject[] buildingAreas)
        {
            for (int i = 0; i < GameManager.Instance.TowerCellList.Count; i++)
            {

                if (cell != GameManager.Instance.TowerCellList[i])
                {
                    if (cell.transform.position == GameManager.Instance.TowerCellList[i].transform.position)
                    {
                        Debug.Log(cell.transform + " = " + GameManager.Instance.TowerCellList[i].transform);
                    }
                }
            }
            

            var spacing = cell.transform.localScale.x + 1;
            var rayDistance = cell.transform.localScale.x;

            var forwardRaycastHit = new RaycastHit();
            var backRaycastHit = new RaycastHit();
            var rightRaycastHit = new RaycastHit();
            var leftRaycastHit = new RaycastHit();

            var buildingAreaLayer = 1 << 8;
            var terrainLayer = 1 << 9;
            var expandLayerMask = ~terrainLayer | buildingAreaLayer;
            var buildLayerMask = ~terrainLayer | buildingAreaLayer;

            var fRay = new Ray(cell.transform.position + Vector3.forward * 15, Vector3.down);
            var bRay = new Ray(cell.transform.position + Vector3.back * 15, Vector3.down);
            var rRay = new Ray(cell.transform.position + Vector3.right * 15, Vector3.down);
            var lRay = new Ray(cell.transform.position + Vector3.left * 15, Vector3.down);

            var rayHit =
                Physics.Raycast(fRay, out forwardRaycastHit, 5, buildLayerMask) |
                Physics.Raycast(bRay, out backRaycastHit, 5, buildLayerMask) |
                Physics.Raycast(rRay, out rightRaycastHit, 5, buildLayerMask) |
                Physics.Raycast(lRay, out leftRaycastHit, 5, buildLayerMask);

            if (rayHit)
            {              
                   FillSide(Vector3.forward, 1, cell, cellPrefab, rayDistance, expandLayerMask, spacing);

                    FillSide(Vector3.back, 1, cell, cellPrefab, rayDistance, expandLayerMask, spacing);

                    FillSide(Vector3.left, 2, cell, cellPrefab, rayDistance, expandLayerMask, spacing);

                   FillSide(Vector3.right, 2, cell, cellPrefab, rayDistance, expandLayerMask, spacing);
                
            }
            else
            {
                GameManager.Instance.TowerCellList.Remove(cell);
                Object.Destroy(cell);
            }


        }
    }
}