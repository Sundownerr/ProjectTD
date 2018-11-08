using UnityEngine;
using Game.Systems;

namespace Game.Cells
{

    public class CellExpandSystem
    {
        private Vector3 forward, back, left, right, down;     
        private int buildLayerMask;

        public CellExpandSystem()
        {
            forward = new Vector3(0, 0, 1);
            back    = new Vector3(0, 0, -1);
            left    = new Vector3(1, 0, 0);
            right   = new Vector3(-1, 0, 0);
            down    = new Vector3(0, -1, 0);

            var results = new RaycastHit[2];

            var buildingAreaLayer   = 1 << 8;
            var terrainLayer        = 1 << 9;
            buildLayerMask          = ~terrainLayer | buildingAreaLayer;          
        }

        public void Expand(Cell ownerCell)
        {
            var spacing = ownerCell.gameObject.transform.localScale.x + 1;
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

            if(isNothingHit || !isDownHit)
            {
                GM.Instance.CellList.Remove(ownerCell.gameObject);
                GM.Instance.CellStateList.Remove(ownerCell);
                Object.Destroy(ownerCell.gameObject);
            } 
            else
            {                      
                if(isForwardHit)
                    Fill(forward, ownerCell, rayDistance, spacing);

                if(isBackHit)
                    Fill(back, ownerCell, rayDistance, spacing);

                if(isLeftHit)
                    Fill(left, ownerCell, rayDistance, spacing);

                if(isRightHit)
                    Fill(right, ownerCell, rayDistance, spacing);         
            }
        }

        private void Fill(Vector3 spawnDirection, Cell ownerCell, float rayDistance, float spacing)
        {
            if (!Physics.Raycast(ownerCell.gameObject.transform.position, spawnDirection, rayDistance, buildLayerMask))
            {
                ownerCell.IsExpanded = true;
                Object.Instantiate(GM.Instance.CellPrefab, ownerCell.gameObject.transform.position + spawnDirection * spacing, Quaternion.identity);    
            }   
        }
    }
}
