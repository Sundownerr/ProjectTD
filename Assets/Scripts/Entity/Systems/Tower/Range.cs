using System.Collections.Generic;
using Game.Creep;
using Game.Systems;
using UnityEngine;

namespace Game.Tower.System
{
    public class Range : ExtendedMonoBehaviour
    {

        public List<CreepSystem> CreepSystemList { get => creepSystemList; set => creepSystemList = value; }
        public List<GameObject> CreepList { get => creepList; set => creepList = value; }

        private List<GameObject> creepList;
        private List<Creep.CreepSystem> creepSystemList;
        private Renderer rend;
        private Color transparent, notTransparent;
        private bool isRangeShowed;

        protected override void Awake()
        {
            base.Awake();

            CreepList = new List<GameObject>();
            CreepSystemList = new List<Creep.CreepSystem>();
            rend = GetComponent<Renderer>();

            transform.position += new Vector3(0, -5, 0);

            transparent = new Color(0f, 0f, 0f, 0f);
            notTransparent = new Color(0, 0.5f, 0, 0.2f);
            isRangeShowed = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < GM.I.CreepList.Count; i++)
                if (other.gameObject == GM.I.CreepList[i])
                {
                    CreepSystemList.Add(other.gameObject.GetComponent<Creep.CreepSystem>());
                    CreepList.Add(other.gameObject);
                }
        }

        private void OnTriggerExit(Collider other)
        {
            if (CreepList.Count > 0)
            {
                CreepSystemList.Remove(other.gameObject.GetComponent<Creep.CreepSystem>());
                CreepList.Remove(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            for (int i = 0; i < CreepList.Count; i++)
                if (CreepList[i] == null)
                {
                    CreepList.RemoveAt(i);
                    CreepSystemList.RemoveAt(i);
                }
        }

        private void Show(bool show)
        {
            isRangeShowed = show ? true : false;
            rend.material.color = show ? notTransparent : transparent;
        }

        public void SetShow()
        {
            var isChoosedTower =
                GM.I.TowerUISystem.gameObject.activeSelf &&
                GM.I.PlayerInputSystem.ChoosedTower == transform.parent.gameObject;

            if (isChoosedTower)
            {
                if (!isRangeShowed)
                    Show(true);
            }
            else if (isRangeShowed)
                Show(false);
        }

        public void SetShow(bool show) => Show(show);
    }
}