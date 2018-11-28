using System;
using System.Collections.Generic;
using Game.Creep;
using Game.Systems;
using Game.Tower.Data;
using Game.Tower.System;
using UnityEngine;
using U = UnityEngine.Object;

namespace Game.Tower
{
    public class TowerSystem : EntitySystem
    {
        public Transform RangeTransform     { get => rangeTransform;        private set => rangeTransform = value; }
        public Transform MovingPart         { get => movingPartTransform;   private set => movingPartTransform = value; }
        public Transform StaticPart         { get => staticPartTransform;   private set => staticPartTransform = value; }
        public Transform ShootPoint         { get => shootPointTransform;   private set => shootPointTransform = value; }
        public GameObject OcuppiedCell      { get => ocuppiedCell;          set => ocuppiedCell = value; }
        public GameObject Bullet            { get => bullet;                private set => bullet = value; }
        public GameObject Range             { get => range;                 private set => range = value; }
        public Range RangeSystem            { get => rangeSystem;           private set => rangeSystem = value; }
        public Special SpecialSystem        { get => specialSystem;         private set => specialSystem = value; }
        public Combat CombatSystem          { get => combatSystem;          private set => combatSystem = value; }
        public AbilityControlSystem AbilitySystem  { get => abilitySystem;         private set => abilitySystem = value; }
        public Stats StatsSystem            { get => statsSystem;           private set => statsSystem = value; }
        public TowerData Stats              { get => StatsSystem.CurrentStats; set => StatsSystem.CurrentStats = value; }
        public Renderer[] RendererList      { get => rendererList;          private set => rendererList = value; }
        public List<CreepSystem> CreepInRangeList => rangeSystem.CreepSystemList;

        public List<AbilitySystem> AbilitySystemList { get => abilitySystemList; set => abilitySystemList = value; }

        private Transform rangeTransform, movingPartTransform, staticPartTransform, shootPointTransform;
        private GameObject ocuppiedCell, bullet, range;
        private Renderer[] rendererList;
        private Range rangeSystem;
        private Special specialSystem;
        private Combat combatSystem;
        private AbilityControlSystem abilitySystem;
        private Stats statsSystem;
        private List<AbilitySystem> abilitySystemList;


        public TowerSystem(GameObject ownerPrefab)
        {         
            prefab = ownerPrefab;
            movingPartTransform = ownerPrefab.transform.GetChild(0);
            staticPartTransform = ownerPrefab.transform.GetChild(1);
            shootPointTransform = MovingPart.GetChild(0).GetChild(0);
            bullet = ownerPrefab.transform.GetChild(2).gameObject;

            statsSystem     = new Stats(this);
            specialSystem   = new Special(this);
            combatSystem    = new Combat(this);
            abilitySystem   = new AbilityControlSystem(this);
            effectSystem    = new AppliedEffectSystem();         
            AbilitySystemList = new List<AbilitySystem>();
         
            bullet.SetActive(false);   
            isVulnerable = false;                           
        }

        public void SetSystem()
        {               
            for (int i = 0; i < Stats.AbilityList.Count; i++)          
                AbilitySystemList.Add(new AbilitySystem(Stats.AbilityList[i], this));   
                
            if (!Stats.IsGradeTower)
            {
                statsSystem.Set();
                specialSystem.Set();
            }
            
            combatSystem.Set();
            abilitySystem.Set();

            range = U.Instantiate(GM.I.RangePrefab, prefab.transform);           
            range.transform.localScale = new Vector3(Stats.Range, 0.001f, Stats.Range);
            rangeSystem = range.GetComponent<System.Range>();
            rangeSystem.Owner = this;    

            RendererList = prefab.GetComponentsInChildren<Renderer>();                       
        }
        
        public void AddExp(int amount) => StatsSystem.AddExp(amount);      
    }
}