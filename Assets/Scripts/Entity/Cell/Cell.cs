using UnityEngine;
using Game.System;
#pragma warning disable CS1591 
namespace Game.TowerCells
{

    public class Cell : ExtendedMonoBehaviour
    {
        [HideInInspector]
        public bool IsBusy, IsChosen;

        private Color blueColor, redColor, greenColor;
        private Renderer cellRenderer;

        private void Start()
        {            
            GameManager.Instance.CellList.Add(gameObject);
            transform.parent = GameManager.Instance.CellParent;

            cellRenderer = GetComponent<Renderer>();
            cellRenderer.material.color = new Color(0, 0, 0, 0);

            redColor = new Color(0.3f, 0.1f, 0.1f, 0.6f);
            greenColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
            blueColor = new Color(0.1f, 0.1f, 0.3f, 0.4f);

            new CellExpandSystem(gameObject, GameManager.Instance.CellPrefab, GameManager.Instance.TowerCellAreaList);
        }

        private void Update()
        {
            if (GameManager.PLAYERSTATE == GameManager.PLAYERSTATE_PLACINGTOWER)
            {
                if (IsBusy)
                {
                    cellRenderer.material.color = redColor;
                }
                else if (IsChosen)
                {
                    cellRenderer.material.color = greenColor;
                }
                else
                {
                    cellRenderer.material.color = blueColor;
                }                  
            }
        }
    }
}
