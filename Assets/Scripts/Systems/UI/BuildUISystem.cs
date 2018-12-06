using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Tower.Data.Stats;
using Game.Tower.Data;
using System;
using TMPro;

namespace Game.Systems
{
    public class BuildUISystem : ExtendedMonoBehaviour
    {
        public List<Button> ElementButtons;
        public List<GameObject> RarityGOs;
        public GameObject TowerButtonPrefab;
        public StateMachine State;
        public ElementType ChoosedElement;
        public GameObject Rarity;
        public bool IsChoosedNewTower;
        public event EventHandler NeedToBuildTower = delegate{};      

        private List<TowerData> availableTowers;      
        private List<GameObject> towerButtonGOs;
        private List<TowerButtonSystem> towerButtons;
        private RectTransform rarityTransform;
        private Vector2 newTowerButtonPos;

        protected override void Awake()
        {
            GM.I.BuildUISystem = this;  
              
            base.Awake();          

            towerButtonGOs   = new List<GameObject>();
            towerButtons     = new List<TowerButtonSystem>();
            availableTowers  = new List<TowerData>();
            newTowerButtonPos   = new Vector2(0, 32);
            availableTowers  = GM.I.AvailableTowers;
            rarityTransform     = Rarity.GetComponent<RectTransform>();       

            ElementButtons[0].onClick.AddListener(ShowAstral);
            ElementButtons[1].onClick.AddListener(ShowDarkness);
            ElementButtons[2].onClick.AddListener(ShowIce);
            ElementButtons[3].onClick.AddListener(ShowIron);
            ElementButtons[4].onClick.AddListener(ShowStorm);
            ElementButtons[5].onClick.AddListener(ShowNature);
            ElementButtons[6].onClick.AddListener(ShowFire);

            UpdateAvailableElement();

            gameObject.SetActive(false);
            Rarity.gameObject.SetActive(false);                        
        }

        private void Start()
        {
            GM.I.PlayerInputSystem.StartedTowerBuild         += UpdateUI;
            GM.I.TowerCreatingSystem.AddedNewAvailableTower  += UpdateUI;
            GM.I.TowerPlaceSystem.TowerStateChanged          += UpdateUI;
            GM.I.TowerPlaceSystem.TowerDeleted               += OnTowerDeleted;
        }

        private void OnTowerDeleted(object sender, TowerEventArgs e) 
        {
            AddTowerButton(e.Stats);
        } 
 
        private void UpdateUI(object sender, EventArgs e)
        {
            UpdateAvailableElement();
            UpdateRarity();
        }

        private void ActivateButtonList(ref List<Button> list, bool active)
        {
            for (int i = 0; i < list.Count; i++)            
                list[i].gameObject.SetActive(active);                                   
        }

        private void UpdateAvailableElement()
        {           
            ActivateButtonList(ref ElementButtons, false);      

            for (int i = 0; i < availableTowers.Count; i++)           
                ElementButtons[(int)availableTowers[i].Element].gameObject.SetActive(true);
        }      

        public void BuildNewTower() => NeedToBuildTower?.Invoke(this, new EventArgs());
        
        private void ShowRarity(ElementType element)
        {           
            ChoosedElement = element;
            Rarity.gameObject.SetActive(true);
            rarityTransform.SetParent(ElementButtons[(int)ChoosedElement].GetComponent<RectTransform>());
            rarityTransform.localPosition = Vector2.zero;

            UpdateRarity();                       
        }

        private void UpdateRarity()
        {                          
            for (int i = 0; i < towerButtons.Count; i++)                      
                towerButtonGOs[i].gameObject.SetActive(towerButtons[i].TowerData.Element == ChoosedElement);                                                                                                    
        }

        public void RemoveTowerButton(TowerButtonSystem towerButton)
        {
            towerButtonGOs.Remove(towerButton.gameObject);
            towerButtons.Remove(towerButton);
            var buttonRects = new List<RectTransform>();

            for (int i = 0; i < towerButtons.Count; i++)          
                if (towerButtons[i].TowerData.Element == towerButton.TowerData.Element)
                    if (towerButtons[i].TowerData.Rarity == towerButton.TowerData.Rarity)                                      
                        buttonRects.Add(towerButtons[i].GetComponent<RectTransform>());

            for (int i = 0; i < buttonRects.Count; i++)              
            {                             
                var isNewButtonPosBusy = false;
                var newButtonPos = (Vector2)buttonRects[i].localPosition - newTowerButtonPos;
                
                for (int j = 0; j < buttonRects.Count; j++)                              
                    if (newButtonPos.y == buttonRects[j].localPosition.y)  
                    {
                        isNewButtonPosBusy = true;
                        break;  
                    }                                     
                if (isNewButtonPosBusy)
                    break;
                else
                    if (newButtonPos.y >= 0)
                        buttonRects[i].localPosition = newButtonPos;            
            }                                                                                                        
        }

        public void AddTowerButton(TowerData towerData)
        {
            var towerCount = 0;               
            var isSameTower = false;
             
            for (int i = 0; i < towerButtons.Count; i++)
            {
                if (towerButtons[i].TowerData == towerData)
                {          
                    isSameTower = true;            
                    AddTowerAmount(i);                                                                     
                }  
            
                if (towerButtons[i].TowerData.Element == towerData.Element)
                    if (towerButtons[i].TowerData.Rarity == towerData.Rarity)
                        towerCount++;
            }

            if (!isSameTower)
                CreateTowerButton();             
            
            #region  Helper functions

            void AddTowerAmount(int index)
            {
                towerButtons[index].Count++;
                towerButtons[index].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = towerButtons[index].Count.ToString();       
            }           

            void CreateTowerButton()
            {
                towerButtonGOs.Add(Instantiate(TowerButtonPrefab, RarityGOs[(int)towerData.Rarity].transform));                      
                towerButtons.Add(towerButtonGOs[towerButtonGOs.Count - 1].GetComponent<TowerButtonSystem>());

                var towerButton = towerButtons[towerButtons.Count - 1];     
                var towerButtonImage = towerButton.gameObject.transform.GetChild(0).GetComponent<Image>();

                towerButton.TowerData = towerData;
                towerButton.GetComponent<RectTransform>().localPosition = newTowerButtonPos * towerCount;
                towerButtonImage.sprite = towerData.Image;
                towerButton.Count = 1;             
            }        

            #endregion
        }

        private void ShowAstral()   => ShowRarity(ElementType.Astral);     
        private void ShowDarkness() => ShowRarity(ElementType.Darkness);                
        private void ShowIce()      => ShowRarity(ElementType.Ice);               
        private void ShowIron()     => ShowRarity(ElementType.Iron);               
        private void ShowStorm()    => ShowRarity(ElementType.Storm);             
        private void ShowNature()   => ShowRarity(ElementType.Nature);        
        private void ShowFire()     => ShowRarity(ElementType.Fire);                 
    }
}