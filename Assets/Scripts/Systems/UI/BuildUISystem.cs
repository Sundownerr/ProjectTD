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
        private Vector2 newTowerButtonPos;

        protected override void Awake()
        {
            GM.Instance.BuildUISystem = this;  
              
            base.Awake();          

            towerButtonGOList   = new List<GameObject>();
            towerButtonList     = new List<TowerButtonSystem>();
            availableTowerList  = new List<TowerData>();
            newTowerButtonPos   = new Vector2(0, 32);
            availableTowerList  = GM.Instance.AvailableTowerList;
            rarityTransform     = Rarity.GetComponent<RectTransform>();       

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
        }

        private void Start()
        {
            GM.Instance.PlayerInputSystem.StartedTowerBuild         += UpdateUI;
            GM.Instance.TowerCreatingSystem.AddedNewAvailableTower  += UpdateUI;
            GM.Instance.TowerPlaceSystem.TowerStateChanged          += UpdateUI;
        }

        public void UpdateUI(object sender, ElementType element)
        {
            UpdateAvailableElement();
            UpdateRarity();
        }

        private void DisableButtonList(ref List<Button> list)
        {
            for (int i = 0; i < list.Count; i++)
                list[i].interactable = false;
        }

        private void UpdateAvailableElement()
        {           
            DisableButtonList(ref ElementButtonList);      

            for (int i = 0; i < availableTowerList.Count; i++)           
                ElementButtonList[(int)availableTowerList[i].Element].interactable = true;
        }      

        public void BuildNewTower() => NeedToBuildTower?.Invoke(this, new EventArgs());
        
        private void ShowRarity()
        {           
            Rarity.gameObject.SetActive(true);
            rarityTransform.SetParent(ElementButtonList[(int)ChoosedElement].GetComponent<RectTransform>());
            rarityTransform.localPosition = Vector2.zero;

            UpdateRarity();                       
        }

        private void UpdateRarity()
        {                          
            DisableButtonList(ref RarityButtonList);

            for (int i = 0; i < towerButtonList.Count; i++) 
            {                        
                var isButtonElementOk = towerButtonList[i].TowerData.Element == ChoosedElement;
                towerButtonGOList[i].gameObject.SetActive(isButtonElementOk);  

                if(isButtonElementOk)                                    
                    RarityButtonList[(int)towerButtonList[i].TowerData.Rarity].interactable = true;                                                                          
            }         
        }

        public void RemoveTowerButton(TowerButtonSystem towerButton)
        {
            towerButtonGOList.Remove(towerButton.gameObject);
            towerButtonList.Remove(towerButton);
            var buttonRectList = new List<RectTransform>();

            for (int i = 0; i < towerButtonList.Count; i++)          
                if(towerButtonList[i].TowerData.Element == towerButton.TowerData.Element)
                    if(towerButtonList[i].TowerData.Rarity == towerButton.TowerData.Rarity)                                      
                        buttonRectList.Add(towerButtonList[i].GetComponent<RectTransform>());

            for (int i = 0; i < buttonRectList.Count; i++)              
            {                             
                var isNewPosBusy = false;
                var newButtonPos = (Vector2)buttonRectList[i].localPosition - newTowerButtonPos;
                
                for (int j = 0; j < buttonRectList.Count; j++)                              
                    if(newButtonPos.y == buttonRectList[j].localPosition.y)  
                    {
                        isNewPosBusy = true;
                        break;  
                    }                       
                 
                if(isNewPosBusy)
                    break;
                else
                    if(newButtonPos.y > 0)
                        buttonRectList[i].localPosition = newButtonPos;            
            }                                                                                                        
        }

        public void AddTowerButton(TowerData towerData)
        {
            var towerCount = 1;               
            var isSameTower = false;
             
            for (int i = 0; i < towerButtonList.Count; i++)
            {
                if (towerButtonList[i].TowerData == towerData)
                {          
                    isSameTower = true;            
                    AddTowerAmount(i);                                                                     
                }  
            
                if(towerButtonList[i].TowerData.Element == towerData.Element)
                    if(towerButtonList[i].TowerData.Rarity == towerData.Rarity)
                        towerCount++;
            }

            if (!isSameTower)
                CreateTowerButton();             
            
            void AddTowerAmount(int index)
            {
                towerButtonList[index].Count++;
                towerButtonList[index].TowerCountText.text = towerButtonList[index].Count.ToString();       
            }            

            void CreateTowerButton()
            {
                towerButtonGOList.Add(Instantiate(TowerButtonPrefab, RarityButtonList[(int)towerData.Rarity].GetComponent<RectTransform>()));                      
                towerButtonList.Add(towerButtonGOList[towerButtonGOList.Count - 1].GetComponent<TowerButtonSystem>());

                var towerButton = towerButtonList[towerButtonList.Count - 1];     
                var towerButtonImage = towerButton.gameObject.transform.GetChild(0).GetComponent<Image>();

                towerButton.TowerData = towerData;
                towerButton.GetComponent<RectTransform>().localPosition = newTowerButtonPos * towerCount;
                towerButtonImage.sprite = towerData.Image;
                towerButton.Count = 1;             
            }        
        }

        private void ShowAstral()
        {
            ChoosedElement = ElementType.Astral;
            ShowRarity();
        }

        private void ShowDarkness()
        {
            ChoosedElement = ElementType.Darkness;
            ShowRarity();            
        }

        private void ShowIce()
        {
            ChoosedElement = ElementType.Ice;
            ShowRarity();        
        }

        private void ShowIron()
        {
            ChoosedElement = ElementType.Iron;
            ShowRarity();         
        }

        private void ShowStorm()
        {
            ChoosedElement = ElementType.Storm;
            ShowRarity();       
        }

        private void ShowNature()
        {
            ChoosedElement = ElementType.Nature;
            ShowRarity();        
        }

        private void ShowFire()
        {
            ChoosedElement = ElementType.Fire;
            ShowRarity();          
        }
    }
}