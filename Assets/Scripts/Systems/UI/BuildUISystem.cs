using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.System
{
    public class BuildUISystem : ExtendedMonoBehaviour
    {
        public List<Button> ButtonElementList;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            ButtonElementList[0].onClick.AddListener(ShowAstral);
            ButtonElementList[1].onClick.AddListener(ShowDarkness);
            ButtonElementList[2].onClick.AddListener(ShowIce);
            ButtonElementList[3].onClick.AddListener(ShowIron);
            ButtonElementList[4].onClick.AddListener(ShowStorm);
            ButtonElementList[5].onClick.AddListener(ShowNature);
            ButtonElementList[6].onClick.AddListener(ShowFire);

            UpdateAvailableElement();


            GM.Instance.BuildUISystem = this;

            gameObject.SetActive(false);
        }

        public void UpdateAvailableElement()
        {
            var availableTowerList = GM.Instance.AvailableTowerList;

            for (int i = 0; i < ButtonElementList.Count; i++)
            {
                ButtonElementList[i].interactable = false;
            }

            for (int i = 0; i < availableTowerList.Count; i++)
            {
                ShowAvailableElement(availableTowerList[i].ElementId);
            }
        }

        public void ShowAvailableElement(int id)
        {
            ButtonElementList[id].interactable = true;
        }

        public void ShowAstral()
        {
           
        }

        public void ShowDarkness()
        {
           
        }

        public void ShowIce()
        {
          
        }

        public void ShowIron()
        {
           
        }

        public void ShowStorm()
        {
          
        }

        public void ShowNature()
        {
           
        }

        public void ShowFire()
        {
          
        }
    }
}