using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Tower.Data.Stats;
using Game.Tower.Data;

namespace Game.Systems
{
    public class BuildUISystem : ExtendedMonoBehaviour
    {
        public List<Button> ElementButtonList, RarityButtonList;
        public GameObject TowerButtonPrefab;
        public StateMachine State;
        public ElementType ChoosedElement;
        public GameObject Rarity;
        public bool IsChoosedNewTower;

        private List<TowerData> availableTowerList;      
        private List<GameObject> towerButtonList;
        private RectTransform rarityTransform;

        private delegate void Act();

        protected override void Awake()
        {
            base.Awake();
            
            towerButtonList = new List<GameObject>();
            availableTowerList = new List<TowerData>();

            ElementButtonList[0].onClick.AddListener(ShowAstral);
            ElementButtonList[1].onClick.AddListener(ShowDarkness);
            ElementButtonList[2].onClick.AddListener(ShowIce);
            ElementButtonList[3].onClick.AddListener(ShowIron);
            ElementButtonList[4].onClick.AddListener(ShowStorm);
            ElementButtonList[5].onClick.AddListener(ShowNature);
            ElementButtonList[6].onClick.AddListener(ShowFire);

            UpdateAvailableElement();

            rarityTransform = Rarity.GetComponent<RectTransform>();

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
                tempList[i].interactable = false;

            return tempList;
        }

        public void UpdateAvailableElement()
        {           
            DisableButtonList(ElementButtonList);

            for (int i = 0; i < availableTowerList.Count; i++)
                ElementButtonList[(int)availableTowerList[i].Element].interactable = true;
        }

        public void ShowRarity(Button elementButton)
        {
            var rarity = Rarity.gameObject;
            
            rarity.SetActive(true);
            rarityTransform.SetParent(elementButton.GetComponent<RectTransform>());
            rarityTransform.localPosition = Vector2.zero;

            for (int i = 0; i < availableTowerList.Count; i++)
                if (availableTowerList[i].Element == ChoosedElement)
                    UpdateRarity(ChoosedElement);
        }

        public void UpdateRarity(ElementType element)
        {      
            
            var towerCount = 0;         

            for (int i = 0; i < towerButtonList.Count; i++)
                Destroy(towerButtonList[i]);           
            towerButtonList.Clear();

            DisableButtonList(RarityButtonList);

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                if (availableTowerList[i].Element == element)
                {
                    RarityButtonList[(int)availableTowerList[i].Rarity].interactable = true;

                    if(towerButtonList.Count == 0)
                        CreateTowerButton(i, towerCount);
                    else
                    {
                        var lastTowerButton = towerButtonList[towerButtonList.Count - 1];
                        var isSameTower = availableTowerList[i] == lastTowerButton.GetComponent<TowerButtonSystem>().TowerData;

                        if (isSameTower)
                            AddTowerCount();
                        else
                            CreateTowerButton(i, towerCount);
                    }

                    towerCount++;
                }
            }
        }

        private void AddTowerCount()
        {
            var towerButton = towerButtonList[towerButtonList.Count - 1];
            var sameTowerCount = towerButton.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;

            sameTowerCount = (int.Parse(sameTowerCount) + 1).ToString();
            towerButton.GetComponent<TowerButtonSystem>().Count++;
        }

        private void CreateTowerButton(int index, int towerCount)
        {
            towerButtonList.Add(Instantiate(TowerButtonPrefab, RarityButtonList[(int)availableTowerList[index].Rarity].GetComponent<RectTransform>()));

            var towerButton = towerButtonList[towerButtonList.Count - 1];
            var towerButtonSystem = towerButton.GetComponent<TowerButtonSystem>();
            var towerButtonImage = towerButton.transform.GetChild(0).GetComponent<Image>().sprite;

            towerButtonSystem.TowerData = availableTowerList[index];
            towerButton.GetComponent<RectTransform>().localPosition = new Vector2(0, 40 * (1 + towerCount));
            towerButtonImage = availableTowerList[index].Image;
            towerButtonSystem.Count++;

            towerButton.SetActive(true);
        }

        public void ShowAstral()
        {
            ChoosedElement = ElementType.Astral;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);
        }

        public void ShowDarkness()
        {
            ChoosedElement = ElementType.Darkness;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);            
        }

        public void ShowIce()
        {
            ChoosedElement = ElementType.Ice;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);        
        }

        public void ShowIron()
        {
            ChoosedElement = ElementType.Iron;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);         
        }

        public void ShowStorm()
        {
            ChoosedElement = ElementType.Storm;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);       
        }

        public void ShowNature()
        {
            ChoosedElement = ElementType.Nature;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);        
        }

        public void ShowFire()
        {
            ChoosedElement = ElementType.Fire;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);          
        }
    }
}