using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#pragma warning disable CS1591 
namespace Game.System
{
    public class PlayerSystem : MonoBehaviour
    {
        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;
        public GameObject ChoosedTower;     
        public LayerMask LayerMask;

        private PointerEventData pointerEventData;
        private List<RaycastResult> results;
        private RaycastHit hit;
        private Ray WorldRay;
        private bool isHitUI;

        private void Start()
        {
            results = new List<RaycastResult>();          
        }

        private void Update()
        {      
            pointerEventData = new PointerEventData(EventSystem)
            {
                position = Input.mousePosition
            };

            if (Input.GetMouseButtonDown(0))
            {
                GraphicRaycaster.Raycast(pointerEventData, results);

                if (results.Count > 0)
                {
                    isHitUI = true;
                }
            }            

            WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(WorldRay, out hit, 10000, LayerMask))
            {

                var isMouseOnTower = hit.transform.gameObject.layer == 14;
                var isMouseNotOnUI = !isHitUI && hit.transform.gameObject.layer == 9;

                if (Input.GetMouseButtonDown(0))
                {
                    if (isMouseOnTower && !Input.GetKey(KeyCode.LeftShift))
                    {
                        ChoosedTower = hit.transform.gameObject;

                        if (!GameManager.Instance.TowerUISystem.gameObject.activeSelf)
                        {
                            GameManager.Instance.TowerUISystem.gameObject.SetActive(true);
                        }

                        StartCoroutine(GameManager.Instance.TowerUISystem.RefreshUI());

                        GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_CHOOSEDTOWER;
                    }

                    if (isMouseNotOnUI)
                    {
                        if (GameManager.Instance.TowerUISystem.gameObject.activeSelf)
                        {
                            GameManager.Instance.TowerUISystem.gameObject.SetActive(false);
                        }

                        if (GameManager.PLAYERSTATE != GameManager.PLAYERSTATE_PLACINGTOWER)
                        {
                            GameManager.PLAYERSTATE = GameManager.PLAYERSTATE_IDLE;
                        }
                    }
                }
            }

            if (results.Count > 0)
            {
                results.Clear();
                isHitUI = false;
            }
        }
    }
}
