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
        public StateMachine State;
        public int ChoosedElementId;
        public GameObject Rarity;

       
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

            gameObject.SetActive(false);
            Rarity.gameObject.SetActive(false);
            TowerButtonPrefab.SetActive(false);

            GM.Instance.BuildUISystem = this;          
        }

        private List<Button> DisableButtonList(List<Button> list)
        {
            var tempList = list;

            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].interactable = false;
            }

            return tempList;
        }

        public void UpdateAvailableElement()
        {
            var availableTowerList = GM.Instance.AvailableTowerList;

            DisableButtonList(ElementButtonList);

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                ElementButtonList[availableTowerList[i].ElementId].interactable = true;
            }
        }

        public void ShowRarity(Button elementButton)
        {
            var availableTowerList = GM.Instance.AvailableTowerList;

            Rarity.gameObject.SetActive(true);
            Rarity.gameObject.GetComponent<RectTransform>().SetParent(elementButton.GetComponent<RectTransform>());
            Rarity.gameObject.GetComponent<RectTransform>().localPosition = Vector2.zero;

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                if (availableTowerList[i].ElementId == ChoosedElementId)
                {
                    UpdateRarity(ChoosedElementId);
                }
            }
        }

        public void UpdateRarity(int elementId)
        {
            var availableTowerList = GM.Instance.AvailableTowerList;
            var towerCount = 0;         

            for (int i = 0; i < towerButtonList.Count; i++)
            {
                Destroy(towerButtonList[i]);           
            }

            towerButtonList.Clear();

            DisableButtonList(RarityButtonList);

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                if (availableTowerList[i].ElementId == elementId)
                {
                    RarityButtonList[availableTowerList[i].RarityId].interactable = true;

                    towerButtonList.Add(Instantiate(TowerButtonPrefab, RarityButtonList[availableTowerList[i].RarityId].GetComponent<RectTransform>()));

                    towerButtonList[towerButtonList.Count - 1].GetComponent<TowerButtonSystem>().TowerData = availableTowerList[i];
                    towerButtonList[towerButtonList.Count - 1].GetComponent<RectTransform>().localPosition = new Vector2(0, 40 * (1 + towerCount));
                    towerButtonList[towerButtonList.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = availableTowerList[i].Image;

                    towerButtonList[towerButtonList.Count - 1].SetActive(true);

                    towerCount++;
                }
            }
        }

        public void ShowAstral()
        {
            ChoosedElementId = 0;
            ShowRarity(ElementButtonList[ChoosedElementId]);
        }

        public void ShowDarkness()
        {
            ChoosedElementId = 1;
            ShowRarity(ElementButtonList[ChoosedElementId]);            
        }

        public void ShowIce()
        {
            ChoosedElementId = 2;
            ShowRarity(ElementButtonList[ChoosedElementId]);        
        }

        public void ShowIron()
        {
            ChoosedElementId = 3;
            ShowRarity(ElementButtonList[ChoosedElementId]);         
        }

        public void ShowStorm()
        {
            ChoosedElementId = 4;
            ShowRarity(ElementButtonList[ChoosedElementId]);       
        }

        public void ShowNature()
        {
            ChoosedElementId = 5;
            ShowRarity(ElementButtonList[ChoosedElementId]);        
        }

        public void ShowFire()
        {
            ChoosedElementId = 6;
            ShowRarity(ElementButtonList[ChoosedElementId]);          
        }
    }
}