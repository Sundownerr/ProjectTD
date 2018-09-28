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

        public static int PLAYERSTATE_IDLE, PLAYERSTATE_PLACINGTOWER, PLAYERSTATE_ChOOSEDCREEP, PLAYERSTATE_CHOOSEDTOWER;
        public LayerMask LayerMask;
        private Ray WorldRay;

        public GraphicRaycaster GraphicRaycaster;
        public EventSystem EventSystem;

        private bool isHitUI;
        private RaycastHit hit;
        private PointerEventData pointerEventData;
        private List<RaycastResult> results;

        private void Start()
        {
            results = new List<RaycastResult>();
        }

        private void LateUpdate()
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
                    Debug.Log(results[0].gameObject);
                    isHitUI = true;
                }
            }            

            WorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(WorldRay, out hit, 10000, LayerMask))
            {

                if (hit.transform.gameObject.layer == 14)
                {
                    if (Input.GetMouseButtonDown(0) && !GameManager.Instance.TowerUISystem.gameObject.activeSelf)
                    {
                        GameManager.Instance.TowerUISystem.gameObject.SetActive(true);
                    }
                }

                if (!isHitUI && hit.transform.gameObject.layer == 9)
                {
                    if (Input.GetMouseButtonDown(0) && GameManager.Instance.TowerUISystem.gameObject.activeSelf)
                    {
                        GameManager.Instance.TowerUISystem.gameObject.SetActive(false);
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
