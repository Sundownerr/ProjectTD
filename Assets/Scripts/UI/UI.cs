using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public Button StartWaveButton, BuildModeButton, ReadyButton;
    public GameObject CreepPrefab, GridManager;
    public bool IsBuildModeActive, IsWaveStarted, IsPlayerReady;


	void Start ()
    {
        Cursor.lockState = CursorLockMode.Confined;

        StartWaveButton.onClick.AddListener(StartWave);
        BuildModeButton.onClick.AddListener(BuildTower);
        ReadyButton.onClick.AddListener(CheckReady);
      
    }
	
    private void StartWave()
    {
        IsWaveStarted = true;
    }

    private void BuildTower()
    {
        if (GridManager.GetComponent<CreateGrid>().IsGridBuilded)
        {
            if (!IsBuildModeActive)           
                IsBuildModeActive = true;
        }
    }

    private void CheckReady()
    {
        IsPlayerReady = true;
        Destroy(ReadyButton.gameObject);
    }

   
}
