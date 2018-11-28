using System.Collections.Generic;
using Game.Creep;
using Game.Systems;
using Game.Tower;
using UnityEngine;

namespace Game
{
    public enum CollideWith
    {
        Creeps,
        Towers,
        CreepsAndTowers
    }
    
    public class Range : ExtendedMonoBehaviour
    {
        public List<EntitySystem> EntitySystemList { get => entitySystemList; set => entitySystemList = value; }
        public List<GameObject> EntityList { get => entityList; set => entityList = value; }
        public TowerSystem Owner { get => owner; set => owner = value; }
        public CollideWith CollideType { get => collideType; set => collideType = value; }

        private List<GameObject> entityList;
        private List<EntitySystem> entitySystemList;
        private Renderer rend;
        private Color transparent, notTransparent;
        private bool isRangeShowed;
        private TowerSystem owner;
        private CollideWith collideType;

        protected override void Awake()
        {
            base.Awake();

            entityList = new List<GameObject>();
            entitySystemList = new List<EntitySystem>();
            rend = GetComponent<Renderer>();

            transform.position += new Vector3(0, -5, 0);

            transparent = new Color(0f, 0f, 0f, 0f);
            notTransparent = new Color(0, 0.5f, 0, 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            AddToList();
          
            void AddToList()
            {
                if(collideType == CollideWith.Creeps)
                    AddEntity<CreepSystem>();
                    
                if(collideType == CollideWith.Towers)
                    AddEntity<TowerSystem>();

                if(collideType == CollideWith.CreepsAndTowers)
                {
                    AddEntity<CreepSystem>();
                    AddEntity<TowerSystem>();
                }
                    
                void AddEntity<T>() where T: EntitySystem
                {
                    if (typeof(T) == typeof(CreepSystem))
                        for (int i = 0; i < GM.I.CreepSystemList.Count; i++)
                            if (CheckFound(GM.I.CreepSystemList[i]))
                                return;

                    if (typeof(T) == typeof(TowerSystem))
                        for (int i = 0; i < GM.I.PlacedTowerList.Count; i++)
                            if (CheckFound(GM.I.PlacedTowerList[i]))
                                return;
                }
                
                bool CheckFound(EntitySystem entitySystem)
                {
                    if (other.gameObject == entitySystem.Prefab)
                    {
                        entitySystemList.Add(entitySystem);
                        entityList.Add(other.gameObject);      
                        return true;                  
                    }       
                    return false;       
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {          
            for (int i = 0; i < entitySystemList.Count; i++)
                if (other.gameObject == entitySystemList[i].Prefab)
                {
                    entitySystemList.Remove(entitySystemList[i]);
                    entityList.Remove(other.gameObject);
                }            
        }

        private void OnTriggerStay(Collider other)
        {
            for (int i = 0; i < entityList.Count; i++)
                if (entityList[i] == null)
                {
                    entityList.RemoveAt(i);
                    entitySystemList.RemoveAt(i);
                }
        }

        private void Show(bool show)
        {
            isRangeShowed = show;
            rend.material.color = show ? notTransparent : transparent;
        }

        public void SetShow()
        {
            var isChoosedTower =
                GM.I.TowerUISystem.gameObject.activeSelf &&
                GM.I.PlayerInputSystem.ChoosedTower == owner;

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