using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCellExpand
{

    private bool FillForwardSide(bool isForwardFilled, GameObject cell, GameObject cellPrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isForwardFilled)
        {
            return true;
        }
        else
        {
            var forwardRayHit = Physics.Raycast(cell.transform.position + Vector3.left * cell.transform.lossyScale.x / 2, Vector3.forward, rayDistance, layerMask) |
                Physics.Raycast(cell.transform.position + Vector3.right * cell.transform.lossyScale.x / 2, Vector3.forward, rayDistance, layerMask);

            if(!forwardRayHit)
            {
                Object.Instantiate(cellPrefab, cell.transform.position + Vector3.forward * spacing, cell.transform.rotation).name = cell.transform.name;
                return true;
            }
        }

        return false;
    }

    private bool FillBackwardSide(bool isBackwardFilled, GameObject cell, GameObject cellPrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isBackwardFilled)
        {
            return true;
        }
        else
        {
            var backRayHit = Physics.Raycast(cell.transform.position + Vector3.left * cell.transform.lossyScale.x / 2, Vector3.back, rayDistance, layerMask) |
                Physics.Raycast(cell.transform.position + Vector3.right * cell.transform.lossyScale.x / 2, Vector3.back, rayDistance, layerMask);

            if (!backRayHit)
            {
                Object.Instantiate(cellPrefab, cell.transform.position + Vector3.back * spacing, cell.transform.rotation).name = cell.transform.name;
                return true;
            }
        }

        return false;
    }

    private bool FillRightSide(bool isRightFilled, GameObject cell, GameObject cellPrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isRightFilled)
        {
            return true;
        }
        else
        {
            var rightRayHit = Physics.Raycast(cell.transform.position + Vector3.forward * cell.transform.lossyScale.x / 2, Vector3.right, rayDistance, layerMask) |
                Physics.Raycast(cell.transform.position + Vector3.back * cell.transform.lossyScale.x / 2, Vector3.right, rayDistance, layerMask);

            if (!rightRayHit)
            {
                Object.Instantiate(cellPrefab, cell.transform.position + Vector3.right * spacing, cell.transform.rotation).name = cell.transform.name;
                return true;
            }
        }

        return false;
    }

    private bool FillLeftSide(bool isLeftFilled, GameObject cell, GameObject cellPrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isLeftFilled)
        {
            return true;
        }
        else
        {
            var leftRayHit = Physics.Raycast(cell.transform.position + Vector3.forward * cell.transform.lossyScale.x / 2, Vector3.left, rayDistance, layerMask) |
                Physics.Raycast(cell.transform.position + Vector3.back * cell.transform.lossyScale.x / 2, Vector3.left, rayDistance, layerMask);

            if (!leftRayHit)
            {
                Object.Instantiate(cellPrefab, cell.transform.position + Vector3.left * spacing, cell.transform.rotation).name = cell.transform.name;
                return true;
            }
        }

        return false;
    }

    public TowerCellExpand(GameObject cell, GameObject cellPrefab, List<GameObject> buildingAreas)
    {
        var forwardFilled = false;
        var backFilled = false;
        var leftFilled = false;
        var rightFilled = false;

        var spacing = cell.transform.localScale.x + 1;
        var rayDistance = cell.transform.localScale.x;

        var raycastHit1 = new RaycastHit();
        var raycastHit2 = new RaycastHit();
        var raycastHit3 = new RaycastHit();
        var raycastHit4 = new RaycastHit();

        var buildingAreaLayer = 1 << 8;
        var terrainLayer = 1 << 9;
        var expandLayerMask = ~terrainLayer | buildingAreaLayer;
        var buildLayerMask = ~terrainLayer | buildingAreaLayer;

        var fRay = new Ray(cell.transform.position + Vector3.forward * 15, Vector3.down);
        var bRay = new Ray(cell.transform.position + Vector3.back * 15, Vector3.down);
        var rRay = new Ray(cell.transform.position + Vector3.right * 15, Vector3.down);
        var lRay = new Ray(cell.transform.position + Vector3.left * 15, Vector3.down);

        var rayHit = Physics.Raycast(fRay, out raycastHit1, 5, buildLayerMask) |
            Physics.Raycast(bRay, out raycastHit2, 5, buildLayerMask) |
            Physics.Raycast(rRay, out raycastHit3, 5, buildLayerMask) |
            Physics.Raycast(lRay, out raycastHit4, 5, buildLayerMask);

        if (rayHit)
        {
            for (int i = 0; i < buildingAreas.Count; i++)
            {
                forwardFilled = FillForwardSide(forwardFilled, cell, cellPrefab, rayDistance, expandLayerMask, spacing);

                backFilled = FillBackwardSide(forwardFilled, cell, cellPrefab, rayDistance, expandLayerMask, spacing);

                leftFilled = FillLeftSide(leftFilled, cell, cellPrefab, rayDistance, expandLayerMask, spacing);

                rightFilled = FillRightSide(rightFilled, cell, cellPrefab, rayDistance, expandLayerMask, spacing);
            }
        }
        else
        {
            GameManager.Instance.TowerCellList.Remove(cell);
            Object.Destroy(cell);
        }
    }
}
