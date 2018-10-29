﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : ExtendedMonoBehaviour
{
    public Button QuitButton, NewGameButton;

    protected override void Awake()
    {
        if ((object)CachedTransform == null)
            CachedTransform = transform;

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
