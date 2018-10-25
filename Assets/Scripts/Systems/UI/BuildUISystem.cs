using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.System
{
    public class BuildUISystem : ExtendedMonoBehaviour
    {
        public List<Button> ElementButtonList, RarityButtonList;
        public GameObject TowerButtonPrefab;
        public Button Tower;

        public GameObject Rarity;

        private int choosedElementId;
        private List<GameObject> towerButtonList;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            towerButtonList = new List<GameObject>();

            ElementButtonList[0].onClick.AddListener(ShowAstral);
            ElementButtonList[1].onClick.AddListener(ShowDarkness);
            ElementButtonList[2].onClick.AddListener(ShowIce);
            ElementButtonList[3].onClick.AddListener(ShowIron);
            ElementButtonList[4].onClick.AddListener(ShowStorm);
            ElementButtonList[5].onClick.AddListener(ShowNature);
            ElementButtonList[6].onClick.AddListener(ShowFire);

            UpdateAvailableElement();

            GM.Instance.BuildUISystem = this;

            gameObject.SetActive(false);
            Rarity.gameObject.SetActive(false);
            TowerButtonPrefab.SetActive(false);
        }

        public void UpdateAvailableElement()
        {
            var availableTowerList = GM.Instance.AvailableTowerList;

            for (int i = 0; i < ElementButtonList.Count; i++)
            {
                ElementButtonList[i].interactable = false;
            }

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                ShowAvailableElement(availableTowerList[i].ElementId);
            }
        }

        public void ShowAvailableElement(int id)
        {
            ElementButtonList[id].interactable = true;
           
        }

        public void ShowRarity(Button elementButton)
        {
            var availableTowerList = GM.Instance.AvailableTowerList;

            Rarity.gameObject.SetActive(true);
            Rarity.gameObject.GetComponent<RectTransform>().SetParent(elementButton.GetComponent<RectTransform>());
            Rarity.gameObject.GetComponent<RectTransform>().localPosition = Vector2.zero;

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                if (availableTowerList[i].ElementId == choosedElementId)
                {
                    UpdateRarity(choosedElementId);
                }
            }
        }

        public void UpdateRarity(int elementId)
        {
            var availableTowerList = GM.Instance.AvailableTowerList;
            var towerCount = 0;

            for (int i = 0; i < RarityButtonList.Count; i++)
            {
                RarityButtonList[i].interactable = false;
            }

            for (int i = 0; i < towerButtonList.Count; i++)
            {
                Destroy(towerButtonList[i]);           
            }

            towerButtonList.Clear();


            for (int i = 0; i < availableTowerList.Count; i++)
            {
                if (availableTowerList[i].ElementId == elementId)
                {
                   
                    RarityButtonList[availableTowerList[i].RarityId].interactable = true;

                    towerButtonList.Add(Instantiate(TowerButtonPrefab, RarityButtonList[availableTowerList[i].RarityId].GetComponent<RectTransform>()));               

                    towerButtonList[towerButtonList.Count - 1].GetComponent<RectTransform>().localPosition = new Vector2(0, 40 * (1 + towerCount));
                    towerButtonList[towerButtonList.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = availableTowerList[i].Image;
                    towerButtonList[towerButtonList.Count - 1].SetActive(true);

                    towerCount++;
                    Debug.Log(towerCount);
                }
            }
        }

        public void ShowAstral()
        {
            choosedElementId = 0;
            ShowRarity(ElementButtonList[choosedElementId]);
        }

        public void ShowDarkness()
        {
            choosedElementId = 1;
            ShowRarity(ElementButtonList[choosedElementId]);            
        }

        public void ShowIce()
        {
            choosedElementId = 2;
            ShowRarity(ElementButtonList[choosedElementId]);        
        }

        public void ShowIron()
        {
            choosedElementId = 3;
            ShowRarity(ElementButtonList[choosedElementId]);         
        }

        public void ShowStorm()
        {
            choosedElementId = 4;
            ShowRarity(ElementButtonList[choosedElementId]);       
        }

        public void ShowNature()
        {
            choosedElementId = 5;
            ShowRarity(ElementButtonList[choosedElementId]);        
        }

        public void ShowFire()
        {
            choosedElementId = 6;
            ShowRarity(ElementButtonList[choosedElementId]);          
        }
    }
}