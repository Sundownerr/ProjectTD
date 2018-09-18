using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public Button StartWaveButton, BuildModeButton, ReadyButton;
    public GameObject CreepPrefab, GridManager;
    public bool BuildModeActive, WaveStarted, PlayerReady;


	void Start ()
    {
        Cursor.lockState = CursorLockMode.Confined;

        StartWaveButton.onClick.AddListener(StartWave);
        BuildModeButton.onClick.AddListener(BuildTower);
        ReadyButton.onClick.AddListener(CheckReady);
      
    }
	
    private void StartWave()
    {
        WaveStarted = true;
    }

    private void BuildTower()
    {
        if (GridManager.GetComponent<CreateGrid>().isGridBuilded)
        {
            if (!BuildModeActive)           
                BuildModeActive = true;
        }
    }

    private void CheckReady()
    {
        PlayerReady = true;
        Destroy(ReadyButton.gameObject);
    }

   
}
