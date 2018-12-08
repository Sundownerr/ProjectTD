using UnityEngine;
using Game.Systems;

namespace Game.Cells
{
    public static class CellExpandSystem
    {
        public static void Expand(Cell ownerCell)
        {
            var forward = new Vector3(0, 0, 1);
            var back    = new Vector3(0, 0, -1);
            var left    = new Vector3(1, 0, 0);
            var right   = new Vector3(-1, 0, 0);
            var down    = new Vector3(0, -1, 0);
            var buildingAreaLayer   = 1 << 8;
            var terrainLayer        = 1 << 9;
            var buildLayerMask      = ~terrainLayer | buildingAreaLayer;     

            var spacing = ownerCell.gameObject.transform.localScale.x;
            var rayDistance = ownerCell.gameObject.transform.localScale.x;

            var results = new RaycastHit[1];

            var forwardRay  = new Ray(ownerCell.gameObject.transform.position + forward * rayDistance, down);
            var backRay     = new Ray(ownerCell.gameObject.transform.position + back * rayDistance, down);
            var rightRay    = new Ray(ownerCell.gameObject.transform.position + right * rayDistance, down);
            var leftRay     = new Ray(ownerCell.gameObject.transform.position + left * rayDistance, down);
            var downRay     = new Ray(ownerCell.gameObject.transform.position, down);
              
            var isForwardHit    = Physics.RaycastNonAlloc(forwardRay, results, 5, buildLayerMask) > 0;
            var isBackHit       = Physics.RaycastNonAlloc(backRay, results, 5, buildLayerMask) > 0;
            var isRightHit      = Physics.RaycastNonAlloc(rightRay, results, 5, buildLayerMask) > 0;
            var isLeftHit       = Physics.RaycastNonAlloc(leftRay, results, 5, buildLayerMask) > 0;  
            var isDownHit       = Physics.RaycastNonAlloc(downRay, results, 15, buildLayerMask) > 0;  

            var isNothingHit = !isForwardHit && !isBackHit && !isLeftHit && isRightHit;

            if (isNothingHit || !isDownHit)
            {
                GM.I.GridSystem.Cells.Remove(ownerCell);
                Object.Destroy(ownerCell.gameObject);
            } 
            else
            {                      
                if (isForwardHit)
                    Fill(forward);

                if (isBackHit)
                    Fill(back);

                if (isLeftHit)
                    Fill(left);

                if (isRightHit)
                    Fill(right);         
            }
            
            #region  Helper functions

            void Fill(Vector3 spawnDirection)
            {
                if (!Physics.Raycast(ownerCell.gameObject.transform.position, spawnDirection, rayDistance, buildLayerMask))
                {
                    ownerCell.IsExpanded = true;
                    Object.Instantiate(GM.I.CellPrefab, ownerCell.gameObject.transform.position + spawnDirection * spacing, Quaternion.identity, GM.I.CellParent);    
                }   
            }

            #endregion
        }
    }
}
