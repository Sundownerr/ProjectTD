using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Tower.Data.Stats;
using Game.Tower.Data;
using System;

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
        public event EventHandler NeedToBuildTower = delegate{};
        

        private List<TowerData> availableTowerList;      
        private List<GameObject> towerButtonGOList;
        private List<TowerButtonSystem> towerButtonList;
        private RectTransform rarityTransform;

        protected override void Awake()
        {
            GM.Instance.BuildUISystem = this;  
              
            base.Awake();          

            towerButtonGOList   = new List<GameObject>();
            towerButtonList     = new List<TowerButtonSystem>();
            availableTowerList  = new List<TowerData>();

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
        }

        private void Start()
        {
            GM.Instance.PlayerInputSystem.StartedTowerBuild += UpdateUI;
            GM.Instance.TowerCreatingSystem.AddedNewAvailableTower += UpdateUI;
            GM.Instance.TowerPlaceSystem.TowerStateChanged += UpdateUI;
        }

        public void UpdateUI(object sender, ElementType element)
        {
            UpdateAvailableElement();
            UpdateRarity(element);
        }

        private void DisableButtonList(ref List<Button> list)
        {
            for (int i = 0; i < list.Count; i++)
                list[i].interactable = false;
        }

        public void UpdateAvailableElement()
        {           
            DisableButtonList(ref ElementButtonList);          

            for (int i = 0; i < availableTowerList.Count; i++)
                ElementButtonList[(int)availableTowerList[i].Element].interactable = true;
        }      

        public void BuildNewTower() => NeedToBuildTower?.Invoke(this, new EventArgs());
        
        public void ShowRarity(Button elementButton)
        {           
            Rarity.gameObject.SetActive(true);
            rarityTransform.SetParent(elementButton.GetComponent<RectTransform>());
            rarityTransform.localPosition = Vector2.zero;

            UpdateRarity(ChoosedElement);                       
        }

        public void UpdateRarity(ElementType element)
        {
            void SetTowerButtonsActive(bool set, int towerIndex)
            {
                var rarityButton = RarityButtonList[(int)availableTowerList[towerIndex].Rarity];
                rarityButton.interactable = set;

                for (int j = 0; j < rarityButton.transform.childCount; j++)
                    rarityButton.transform.GetChild(j).gameObject.SetActive(set);
            }

            for (int i = 0; i < availableTowerList.Count; i++)
                SetTowerButtonsActive(availableTowerList[i].Element == element, i);          
        }

        public void RemoveTowerButton(TowerButtonSystem towerButton)
        {
            towerButtonGOList.Remove(towerButton.gameObject);
            towerButtonList.Remove(towerButton);
        }

        public void AddTowerButton(TowerData towerData)
        {
            var towerCount = 0;

            void AddTowerAmount(int index)
            {
                var towerButton = towerButtonList[index];  
                var towerButtonText = towerButton.gameObject.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
                
                towerButton.Count++;
                towerButtonText.text = towerButton.Count.ToString();       
            }

            for (int i = 0; i < availableTowerList.Count; i++)                                   
                if(towerData.Element == availableTowerList[i].Element)
                {                 
                    var isSameTower = false;
                        
                    for (int j = 0; j < towerButtonList.Count; j++)
                        if(towerData == towerButtonList[j].TowerData)
                        {                      
                            isSameTower = true; 
                            AddTowerAmount(j);                                                   
                            break;
                        }                     
                    
                    if(!isSameTower)
                        CreateTowerButton(towerData, towerCount);  

                    if(towerData != availableTowerList[i])
                        towerCount++;
                }                                   
        }

        private void CreateTowerButton(TowerData towerData, int towerCount)
        {
            towerButtonGOList.Add(Instantiate(TowerButtonPrefab, RarityButtonList[(int)towerData.Rarity].GetComponent<RectTransform>()));                      
            towerButtonList.Add(towerButtonGOList[towerButtonGOList.Count - 1].GetComponent<TowerButtonSystem>());

            var towerButton = towerButtonList[towerButtonList.Count - 1];     
            var towerButtonImage = towerButton.gameObject.transform.GetChild(0).GetComponent<Image>();

            towerButton.TowerData = towerData;
            towerButton.GetComponent<RectTransform>().localPosition = new Vector2(0, 33 * (1 + towerCount));
            towerButtonImage.sprite = towerData.Image;
            towerButton.Count = 1;
        }

        private void ShowAstral()
        {
            ChoosedElement = ElementType.Astral;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);
        }

        private void ShowDarkness()
        {
            ChoosedElement = ElementType.Darkness;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);            
        }

        private void ShowIce()
        {
            ChoosedElement = ElementType.Ice;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);        
        }

        private void ShowIron()
        {
            ChoosedElement = ElementType.Iron;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);         
        }

        private void ShowStorm()
        {
            ChoosedElement = ElementType.Storm;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);       
        }

        private void ShowNature()
        {
            ChoosedElement = ElementType.Nature;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);        
        }

        private void ShowFire()
        {
            ChoosedElement = ElementType.Fire;
            ShowRarity(ElementButtonList[(int)ChoosedElement]);          
        }
    }
}