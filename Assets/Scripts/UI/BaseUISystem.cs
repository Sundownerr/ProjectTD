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
        public bool IsBuildModeActive, IsWaveStarted, IsPlayerReady;


        void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;

            StartWaveButton.onClick.AddListener(StartWave);
            BuildModeButton.onClick.AddListener(BuildTower);
            ReadyButton.onClick.AddListener(CheckReady);
        }

        private void StartWave()
        {
            if (GameManager.Instance.CreepList.Count == 0)
            {
                IsWaveStarted = true;
                StartWaveButton.gameObject.SetActive(false);              
            }
        }

        private void BuildTower()
        {
            if (GameManager.Instance.GridSystem.IsGridBuilded)
            {
                if (!IsBuildModeActive)
                {
                    IsBuildModeActive = true;
                }
            }
        }

        private void CheckReady()
        {
            IsPlayerReady = true;
            Destroy(ReadyButton.gameObject);
        }      

        private void Update()
        {
            if (GameManager.Instance.GridSystem.IsGridBuilded)
            {
                if (IsBuildModeActive)
                {
                    if (!GameManager.Instance.TowerCellList[GameManager.Instance.TowerCellList.Count - 1].activeSelf)
                    {
                        for (int i = 0; i < GameManager.Instance.TowerCellList.Count; i++)
                        {
                            GameManager.Instance.TowerCellList[i].SetActive(true);
                        }
                    }
                }

                if (!IsBuildModeActive)
                {
                    if (GameManager.Instance.TowerCellList[GameManager.Instance.TowerCellList.Count - 1].activeSelf)
                    {
                        for (int i = 0; i < GameManager.Instance.TowerCellList.Count; i++)
                        {
                            GameManager.Instance.TowerCellList[i].SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
