using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.TowerCells
{

    public class TowerCell : ExtendedMonoBehaviour
    {
        public bool IsBusy, IsChosen;

        private Color blueColor, redColor, greenColor;
        private Renderer cellRenderer;

        private void Start()
        {
            GameManager.Instance.TowerCellList.Add(gameObject);
            transform.parent = GameManager.Instance.TowerCellParent;

            cellRenderer = GetComponent<Renderer>();

            new TowerCellExpandSystem(gameObject, GameManager.Instance.TowerCellPrefab, GameManager.Instance.TowerCellAreaList);

            redColor = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            greenColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blueColor = new Color(0.1f, 0.1f, 0.3f, 0.4f);
        }

        private void Update()
        {
            
            if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
            {
                
                cellRenderer.material.color = blueColor;

                if (IsBusy)
                {
                    cellRenderer.material.color = redColor;
                    IsChosen = false;
                }

                if (IsChosen)
                {
                    cellRenderer.material.color = greenColor;
                }
            }
        }
    }
}
