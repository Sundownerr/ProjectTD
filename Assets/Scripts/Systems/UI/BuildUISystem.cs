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
        public StateMachine State;
        public int ChoosedElementId;
        public GameObject Rarity;
        public bool IsChoosedNewTower;

        private List<Data.Entity.Tower.TowerData> availableTowerList;

       
        private List<GameObject> towerButtonList;     

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            towerButtonList = new List<GameObject>();
            availableTowerList = new List<Data.Entity.Tower.TowerData>();

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

            availableTowerList = GM.Instance.AvailableTowerList;
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
            DisableButtonList(ElementButtonList);

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                ElementButtonList[availableTowerList[i].ElementId].interactable = true;
            }
        }

        public void ShowRarity(Button elementButton)
        {
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

                    if(towerButtonList.Count > 0)
                    {
                        var lastTowerButton = towerButtonList[towerButtonList.Count - 1];

                        var isSameTower =
                            availableTowerList[i] == lastTowerButton.GetComponent<TowerButtonSystem>().TowerData;

                        if (isSameTower)
                        {
                            AddTowerCount();
                        }
                        else
                        {
                            CreateTowerButton(i, towerCount);
                        }
                    }
                    else
                    {
                        CreateTowerButton(i, towerCount);
                    }
                   
                    towerCount++;
                }
            }
        }


        private void AddTowerCount()
        {
            var lastTowerButton = towerButtonList[towerButtonList.Count - 1];
            var sameTowerCount = lastTowerButton.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;

            lastTowerButton.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = (int.Parse(sameTowerCount) + 1).ToString();
            lastTowerButton.GetComponent<TowerButtonSystem>().Count++;
        }

        private void CreateTowerButton(int index, int towerCount)
        {
            towerButtonList.Add(Instantiate(TowerButtonPrefab, RarityButtonList[availableTowerList[index].RarityId].GetComponent<RectTransform>()));
            var lastTowerButton = towerButtonList[towerButtonList.Count - 1];

            lastTowerButton.GetComponent<TowerButtonSystem>().TowerData = availableTowerList[index];
            lastTowerButton.GetComponent<RectTransform>().localPosition = new Vector2(0, 40 * (1 + towerCount));
            lastTowerButton.transform.GetChild(0).GetComponent<Image>().sprite = availableTowerList[index].Image;
            lastTowerButton.GetComponent<TowerButtonSystem>().Count++;

            lastTowerButton.SetActive(true);
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