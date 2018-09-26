using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public Button QuitButton, NewGameButton;
    
	void Start ()
    {
        QuitButton.onClick.AddListener(QuitClick);
        NewGameButton.onClick.AddListener(NewGameClick);
    }
	
    private void QuitClick()
    {
        Application.Quit();
    }

    private void NewGameClick()
    {
        SceneManager.LoadScene("Game");
    }
}
