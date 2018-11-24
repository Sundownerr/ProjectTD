using System.Collections;
using System.Collections.Generic;
using Game.Cells;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class TowerControlSystem 
	{		
		private List<TowerSystem> towerSystemList = new List<TowerSystem>();

        public void SetSystem()
        {
            GM.I.TowerPlaceSystem.TowerPlaced += OnTowerCreated;
            GM.I.PlayerInputSystem.TowerUpgraded += OnTowerCreated;
            GM.I.TowerPlaceSystem.TowerDeleted += OnTowerDeleted;
            GM.I.PlayerInputSystem.TowerSold += OnTowerDeleted;
        }

        private void OnTowerCreated(object sender, TowerEventArgs e) => AddTower(e.System);
        private void OnTowerDeleted(object sender, TowerEventArgs e) => RemoveTower(e.System);
        
		private void AddTower(TowerSystem tower) 
        {           
            towerSystemList.Add(tower);
            GM.I.PlacedTowerList.Add(tower);  
            tower.OcuppiedCell.GetComponent<Cell>().IsBusy = true;
            tower.Prefab.layer = 14;  
            tower.IsOn = true;
            tower.IsVulnerable = false;			
        }

        private void RemoveTower(TowerSystem tower)
        {
            if(tower.OcuppiedCell != null)
                tower.OcuppiedCell.GetComponent<Cell>().IsBusy = false;
                
            GM.I.PlacedTowerList.Remove(tower);
            tower.Stats.Destroy();
            Object.Destroy(tower.Prefab);
        }
		
        public void UpdateSystem()
        {           
            for (int i = 0; i < towerSystemList.Count; i++)
            {
                var tower = towerSystemList[i];
                if(tower == null)
                    towerSystemList.Remove(tower);
                else
                {                        
                    tower.RangeSystem.SetShow();
                    if (tower.IsOn)                                       
                    {                  
                        tower.AbilitySystem.Update();

                        if (tower.CreepInRangeList.Count < 1)
                        {
                            if (!tower.CombatSystem.CheckAllBulletInactive())
                                tower.CombatSystem.MoveBullet();                   
                        }
                        else
                        {                  
                            tower.CombatSystem.UpdateSystem();                            
                            
                            if (tower.CreepInRangeList[0] != null && tower.CreepInRangeList[0].Prefab != null)
                                RotateAtCreep();
                            
                            for (int j = 0; j < tower.CreepInRangeList.Count; j++)
                                if (tower.CreepInRangeList[j] == null)
                                {
                                    tower.RangeSystem.CreepList.RemoveAt(j);
                                    tower.RangeSystem.CreepSystemList.RemoveAt(j);
                                }

                            void RotateAtCreep()
                            {
                                var offset = tower.CreepInRangeList[0].Prefab.transform.position - tower.Prefab.transform.position;
                                offset.y = 0;
                                tower.MovingPart.rotation = Quaternion.Lerp(tower.MovingPart.rotation, 
                                                                                Quaternion.LookRotation(offset), 
                                                                                Time.deltaTime * 9f);
                            }        
                        }    
                    }        
                }
            }                                             
        }      
    }
}
		
