using System.Collections;
using System.Collections.Generic;
using Game.Cells;
using Game.Creep;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class TowerControlSystem 
	{		
		private List<TowerSystem> towers = new List<TowerSystem>();

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
            towers.Add(tower);
            GM.I.Towers.Add(tower);  
            tower.OcuppiedCell.GetComponent<Cell>().IsBusy = true;
            tower.Prefab.layer = 14;  
            tower.IsOn = true;
            tower.IsVulnerable = false;			
        }

        private void RemoveTower(TowerSystem tower)
        {
            if (tower.OcuppiedCell != null)
                tower.OcuppiedCell.GetComponent<Cell>().IsBusy = false;

            towers.Remove(tower);
            GM.I.Towers.Remove(tower);        
            Object.Destroy(tower.Prefab);
        }
		
        public void UpdateSystem()
        {           
            for (int i = 0; i < towers.Count; i++)
            {
                var tower = towers[i];
                if (tower == null)
                    towers.Remove(tower);
                else
                {                        
                    tower.RangeSystem.SetShow();
                    if (tower.IsOn)                                       
                    {                  
                        tower.AbilityControlSystem.UpdateSystem();
               
                        if (tower.CreepsInRange.Count < 1)                                                 
                            tower.CombatSystem.MoveBullet();                                     
                        else
                        {   
                            tower.CombatSystem.UpdateSystem();    

                            if (tower.CreepsInRange[0] != null && tower.CreepsInRange[0].Prefab != null)                                                           
                                RotateAtCreep();                     
                            
                            for (int j = 0; j < tower.CreepsInRange.Count; j++)
                                if (tower.CreepsInRange[j] == null || tower.CreepsInRange[j].Prefab == null)
                                {
                                    tower.RangeSystem.Entities.RemoveAt(j);
                                    tower.RangeSystem.EntitySystems.RemoveAt(j);
                                }
                                
                            #region  Helper functions

                            void RotateAtCreep()
                            {
                                var offset = tower.CreepsInRange[0].Prefab.transform.position - tower.Prefab.transform.position;
                                offset.y = 0;
                                tower.MovingPart.rotation = 
                                    Quaternion.Lerp(
                                        tower.MovingPart.rotation, 
                                        Quaternion.LookRotation(offset), 
                                        Time.deltaTime * 9f);
                            }    

                            #endregion 
                        }    
                    }        
                }
            }                                             
        }      
    }
}
		
