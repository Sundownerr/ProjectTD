using System;
using System.Collections.Generic;
using Game.Creep;
using Game.Systems;
using Game.Tower.Data;
using Game.Tower.System;
using UnityEngine;

namespace Game.Tower
{
    public class TowerSystem : EntitySystem
    {
        public Transform RangeTransform         { get => rangeTransform; set => rangeTransform = value; }
        public Transform MovingPartTransform    { get => movingPartTransform; set => movingPartTransform = value; }
        public Transform StaticPartTransform    { get => staticPartTransform; set => staticPartTransform = value; }
        public Transform ShootPointTransform    { get => shootPointTransform; set => shootPointTransform = value; }
        public GameObject OcuppiedCell          { get => ocuppiedCell; set => ocuppiedCell = value; }
        public GameObject Bullet            { get => bullet; set => bullet = value; }
        public GameObject Range             { get => range; set => range = value; }
        public Range RangeSystem            { get => rangeSystem; private set => rangeSystem = value; }
        public Special SpecialSystem        { get => specialSystem; set => specialSystem = value; }
        public Combat CombatSystem          { get => combatSystem; private set => combatSystem = value; }
        public AbilitySystem AbilitySystem  { get => abilitySystem; private set => abilitySystem = value; }
        public Stats StatsSystem            { get => statsSystem;  private set => statsSystem = value; }
        public TowerData Stats              { get => StatsSystem.CurrentStats; set => StatsSystem.CurrentStats = value; }
        public Renderer[] RendererList      { get => rendererList; set => rendererList = value; }
        public bool IsTowerPlaced           { get => isTowerPlaced; set => isTowerPlaced = value; }
        public List<CreepSystem> CreepInRangeList => rangeSystem.CreepSystemList;

        private Transform rangeTransform, movingPartTransform, staticPartTransform, shootPointTransform;
        private GameObject ocuppiedCell, bullet, target, range;
        private Renderer[] rendererList;
        private Range rangeSystem;
        private Special specialSystem;
        private Combat combatSystem;
        private AbilitySystem abilitySystem;
        private Stats statsSystem;
        private bool isTowerPlaced;

        protected override void Awake()
        {
            base.Awake();

            movingPartTransform = transform.GetChild(0);
            staticPartTransform = transform.GetChild(1);
            shootPointTransform = MovingPartTransform.GetChild(0).GetChild(0);
            bullet = transform.GetChild(2).gameObject;

            statsSystem     = new Stats(this);
            specialSystem   = new Special(this);
            combatSystem    = new Combat(this);
            abilitySystem   = new AbilitySystem(this);
            effectSystem    = new EffectSystem();         

            
            bullet.SetActive(false);   

            isVulnerable = false;                           
        }

        public void SetSystem()
        {
            statsSystem.Set();
            specialSystem.Set();
            combatSystem.Set();
            abilitySystem.Set();

            range = Instantiate(GM.I.RangePrefab, transform);           
            range.transform.localScale = new Vector3(Stats.Range, 0.001f, Stats.Range);
            rangeSystem = range.GetComponent<System.Range>();

            RendererList = GetComponentsInChildren<Renderer>();
                  
                           
        }
        
        public void AddExp(int amount) => StatsSystem.AddExp(amount);      
    }
}