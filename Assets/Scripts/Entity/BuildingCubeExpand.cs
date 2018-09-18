using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCubeExpand
{

    private bool FillForwardSide(bool isForwardFilled, GameObject cube, GameObject cubePrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isForwardFilled)
        {
            return true;
        }
        else
        {
            var forwardRayHit = Physics.Raycast(cube.transform.position + Vector3.left * cube.transform.lossyScale.x / 2, Vector3.forward, rayDistance, layerMask) |
                Physics.Raycast(cube.transform.position + Vector3.right * cube.transform.lossyScale.x / 2, Vector3.forward, rayDistance, layerMask);

            if(!forwardRayHit)
            {
                Object.Instantiate(cubePrefab, cube.transform.position + Vector3.forward * spacing, cube.transform.rotation).name = cube.transform.name;
                return true;
            }
        }

        return false;
    }

    private bool FillBackwardSide(bool isBackwardFilled, GameObject cube, GameObject cubePrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isBackwardFilled)
        {
            return true;
        }
        else
        {
            var backRayHit = Physics.Raycast(cube.transform.position + Vector3.left * cube.transform.lossyScale.x / 2, Vector3.back, rayDistance, layerMask) |
                Physics.Raycast(cube.transform.position + Vector3.right * cube.transform.lossyScale.x / 2, Vector3.back, rayDistance, layerMask);

            if (!backRayHit)
            {
                Object.Instantiate(cubePrefab, cube.transform.position + Vector3.back * spacing, cube.transform.rotation).name = cube.transform.name;
                return true;
            }
        }

        return false;
    }

    private bool FillRightSide(bool isRightFilled, GameObject cube, GameObject cubePrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isRightFilled)
        {
            return true;
        }
        else
        {
            var rightRayHit = Physics.Raycast(cube.transform.position + Vector3.forward * cube.transform.lossyScale.x / 2, Vector3.right, rayDistance, layerMask) |
                Physics.Raycast(cube.transform.position + Vector3.back * cube.transform.lossyScale.x / 2, Vector3.right, rayDistance, layerMask);

            if (!rightRayHit)
            {
                Object.Instantiate(cubePrefab, cube.transform.position + Vector3.right * spacing, cube.transform.rotation).name = cube.transform.name;
                return true;
            }
        }

        return false;
    }

    private bool FillLeftSide(bool isLeftFilled, GameObject cube, GameObject cubePrefab, float rayDistance, int layerMask, float spacing)
    {
        if (isLeftFilled)
        {
            return true;
        }
        else
        {
            var leftRayHit = Physics.Raycast(cube.transform.position + Vector3.forward * cube.transform.lossyScale.x / 2, Vector3.left, rayDistance, layerMask) |
                Physics.Raycast(cube.transform.position + Vector3.back * cube.transform.lossyScale.x / 2, Vector3.left, rayDistance, layerMask);

            if (!leftRayHit)
            {
                Object.Instantiate(cubePrefab, cube.transform.position + Vector3.left * spacing, cube.transform.rotation).name = cube.transform.name;
                return true;
            }
        }

        return false;
    }

    public BuildingCubeExpand(GameObject cube, GameObject cubePrefab, List<GameObject> buildingAreas)
    {
        var forwardFilled = false;
        var backFilled = false;
        var leftFilled = false;
        var rightFilled = false;

        var spacing = cube.transform.localScale.x + 1;
        var rayDistance = cube.transform.localScale.x;

        var raycastHit1 = new RaycastHit();
        var raycastHit2 = new RaycastHit();
        var raycastHit3 = new RaycastHit();
        var raycastHit4 = new RaycastHit();

        var buildingAreaLayer = 1 << 8;
        var terrainLayer = 1 << 9;
        var expandLayerMask = ~terrainLayer | buildingAreaLayer;
        var buildLayerMask = ~terrainLayer | buildingAreaLayer;

        var fRay = new Ray(cube.transform.position + Vector3.forward * 15, Vector3.down);
        var bRay = new Ray(cube.transform.position + Vector3.back * 15, Vector3.down);
        var rRay = new Ray(cube.transform.position + Vector3.right * 15, Vector3.down);
        var lRay = new Ray(cube.transform.position + Vector3.left * 15, Vector3.down);

        var rayHit = Physics.Raycast(fRay, out raycastHit1, 5, buildLayerMask) |
            Physics.Raycast(bRay, out raycastHit2, 5, buildLayerMask) |
            Physics.Raycast(rRay, out raycastHit3, 5, buildLayerMask) |
            Physics.Raycast(lRay, out raycastHit4, 5, buildLayerMask);

        if (rayHit)
        {
            for (int i = 0; i < buildingAreas.Count; i++)
            {
                forwardFilled = FillForwardSide(forwardFilled, cube, cubePrefab, rayDistance, expandLayerMask, spacing);

                backFilled = FillBackwardSide(forwardFilled, cube, cubePrefab, rayDistance, expandLayerMask, spacing);

                leftFilled = FillLeftSide(leftFilled, cube, cubePrefab, rayDistance, expandLayerMask, spacing);

                rightFilled = FillRightSide(rightFilled, cube, cubePrefab, rayDistance, expandLayerMask, spacing);
            }
        }
        else
            Object.Destroy(cube);
    }
}
