
using UnityEngine;

public class TowerCell : MonoBehaviour
{  
    public bool IsBusy, IsChosen;

    private Color blueColor, redColor, greenColor;
    private Renderer cellRenderer;

    private bool isTransparent;

    private void Start ()
    {
       

        GameManager.Instance.TowerCellList.Add(gameObject);
        gameObject.transform.parent = GameManager.Instance.TowerCellParent.transform;
                  
        cellRenderer = GetComponent<Renderer>();

        var towerCellExpand = new TowerCellExpand(gameObject, GameManager.Instance.TowerCellPrefab, GameManager.Instance.TowerCellAreaList);

        redColor = new Color(0.3f, 0.1f, 0.1f, 0.6f);
        greenColor = new Color(0.1f, 0.3f, 0.1f, 0.5f);
        blueColor = new Color(0.1f, 0.1f, 0.3f, 0.4f);
    }

    private void Update()
    {
        if (GameManager.Instance.UISystem.IsBuildModeActive)
        {                     
            cellRenderer.material.color = blueColor;        

            if (IsBusy)
            {
                cellRenderer.material.color = redColor;
                IsChosen = false;
            }
           
            if (IsChosen)       
                cellRenderer.material.color = greenColor;           
        }       
    }
}
