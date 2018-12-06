using System;
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

    public class EntityEventArgs
    {
        public EntitySystem Entity { get => entity; set => entity = value; }
        
        private EntitySystem entity;     

        public EntityEventArgs(EntitySystem entity)
        {
            this.entity = entity;
        }    
    }

    public class Range : ExtendedMonoBehaviour
    {
        public List<EntitySystem> EntitySystems { get => entitySystems; set => entitySystems = value; }
        public List<GameObject> Entities { get => entities; set => entities = value; }
        public TowerSystem Owner { get => owner; set => owner = value; }
        public CollideWith CollideType { get => collideType; set => collideType = value; }
        public event EventHandler<EntityEventArgs> EntityEntered = delegate{};
        public event EventHandler<EntityEventArgs> EntityExit = delegate{};

        private List<GameObject> entities;
        private List<EntitySystem> entitySystems;
        private Renderer rend;
        private Color transparent, notTransparent;
        private bool isRangeShowed;
        private TowerSystem owner;
        private CollideWith collideType;

        protected override void Awake()
        {
            base.Awake();

            entities = new List<GameObject>();
            entitySystems = new List<EntitySystem>();
            rend = GetComponent<Renderer>();

            transform.position += new Vector3(0, -5, 0);

            transparent = new Color(0f, 0f, 0f, 0f);
            notTransparent = new Color(0, 0.5f, 0, 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            AddToList();

            #region  Helper functions

            void AddToList()
            {
                if (collideType == CollideWith.Creeps)
                    AddEntity<CreepSystem>();
                    
                if (collideType == CollideWith.Towers)
                    AddEntity<TowerSystem>();

                if (collideType == CollideWith.CreepsAndTowers)
                {
                    AddEntity<CreepSystem>();
                    AddEntity<TowerSystem>();
                }
                    
                #region  Helper functions

                void AddEntity<T>() where T: EntitySystem
                {
                    if (typeof(T) == typeof(CreepSystem))
                        for (int i = 0; i < GM.I.Creeps.Count; i++)
                            if (CheckFound(GM.I.Creeps[i]))
                                return;

                    if (typeof(T) == typeof(TowerSystem))
                        for (int i = 0; i < GM.I.Towers.Count; i++)
                            if (CheckFound(GM.I.Towers[i]))
                                return;
                }
                
                bool CheckFound(EntitySystem entitySystem)
                {
                    if (other.gameObject == entitySystem.Prefab)
                    {
                        entitySystems.Add(entitySystem);
                        entities.Add(entitySystem.Prefab);      
                        EntityEntered?.Invoke(this, new EntityEventArgs(entitySystem));
                        return true;                  
                    }       
                    return false;       
                }

                #endregion
            }

            #endregion
        }

        private void OnTriggerExit(Collider other)
        {          
            for (int i = 0; i < entitySystems.Count; i++)
                if (other.gameObject == entitySystems[i].Prefab)
                {
                    EntityExit?.Invoke(this, new EntityEventArgs(entitySystems[i]));
                    entitySystems.Remove(entitySystems[i]);
                    entities.Remove(other.gameObject);
                }            
        }

        private void OnTriggerStay(Collider other)
        {
            for (int i = 0; i < entitySystems.Count; i++)
                if (entitySystems[i] == null || entitySystems[i].Prefab == null )
                {
                    entities.RemoveAt(i);
                    entitySystems.RemoveAt(i);
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