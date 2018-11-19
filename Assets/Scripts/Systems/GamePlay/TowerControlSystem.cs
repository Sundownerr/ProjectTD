using System.Collections;
using System.Collections.Generic;
using Game.Tower;
using UnityEngine;

namespace Game.Systems
{
	public class TowerControlSystem 
	{		
		private List<TowerSystem> towerSystemList = new List<TowerSystem>();

		public void AddTower(TowerSystem tower) => towerSystemList.Add(tower);			
		
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
                        if (tower.IsTowerPlaced)          
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
                                
                                if (tower.CreepInRangeList[0] != null)
                                    RotateAtCreep();
                                
                                for (int j = 0; j < tower.CreepInRangeList.Count; j++)
                                    if (tower.CreepInRangeList[j] == null)
                                    {
                                        tower.RangeSystem.CreepList.RemoveAt(j);
                                        tower.RangeSystem.CreepSystemList.RemoveAt(j);
                                    }

                                void RotateAtCreep()
                                {
                                    var offset = tower.CreepInRangeList[0].gameObject.transform.position - tower.transform.position;
                                    offset.y = 0;
                                    tower.MovingPartTransform.rotation = Quaternion.Lerp(tower.MovingPartTransform.rotation, 
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
		
