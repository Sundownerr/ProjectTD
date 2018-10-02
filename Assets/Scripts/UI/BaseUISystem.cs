using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
#pragma warning disable CS1591 
namespace Game.System
{
    public class BaseUISystem : MonoBehaviour
    {
        public Button StartWaveButton, BuildModeButton, ReadyButton;
        public bool IsWaveStarted, IsPlayerReady;
        public int WaveTimer;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;

            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            ReadyButton.onClick.AddListener(CheckReady);
        }

        private void StartWave()
        {
            if (GameManager.Instance.CreepList.Count == 0 && !IsWaveStarted)
            {
                IsWaveStarted = true;
                StartWaveButton.gameObject.SetActive(false);               
            }
        }

        private void BuildTower()
        {
            if (GameManager.Instance.GridSystem.IsGridBuilded)
            {
                GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_PLACINGTOWER;
            }
        }

        private void CheckReady()
        {
            IsPlayerReady = true;
            Destroy(ReadyButton.gameObject);
        }      

    }
}
