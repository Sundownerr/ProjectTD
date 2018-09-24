using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject Bullet;
    public bool IsTowerBuilded;

    private Transform towerRangeTransform, towerTransform;
    private List<Renderer> towerRendererList;
    private List<GameObject> bullets, bulletsToRemove;     
    private RangeCollider rangeCollider;
    private float timer;
    
    private void StartTowerBuild()
    {
      

        for (int i = 0; i < towerRendererList.Count; i++)
        {
            towerRendererList[i].material.color = GameManager.Instance.TowerPlaceSystem.GhostedTowerColor;
        }
    }

    private bool EndTowerBuild()
    {
        for (int i = 0; i < towerRendererList.Count; i++)
        {
            towerRendererList[i].material.color = Color.blue;
        }
       
        towerRangeTransform.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);

        return true;
    }

    private void RotateTowerAtCreep()
    {
        var offset = rangeCollider.CreepInRangeList[0].transform.position - towerTransform.position;
        offset.y = 0;

        var towerRotation = Quaternion.LookRotation(offset);

        towerTransform.rotation = Quaternion.Lerp(towerTransform.rotation, towerRotation, Time.deltaTime * 5f);
    }

    private void RotateTowerToDefault()
    {
        if (towerTransform.rotation != Quaternion.Euler(0, 0, 0))
        {
            towerTransform.rotation = Quaternion.Lerp(towerTransform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 1f);
        }
    }

    private void ShootAtCreep(float delay)
    {    
        if (timer < delay)
        {
            timer += 0.5f;

            if (timer == 0.5f)
            {
                bullets.Add(Instantiate(Bullet, towerTransform.position, Quaternion.Euler(0, 0, 0)));            
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                if (Vector3.Distance(bullets[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position) > 35)
                {
                    bullets[i].transform.position = Vector3.Lerp(bullets[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position, Time.deltaTime * 10f);
                }
                else
                {
                    bullets[i].GetComponent<ParticleSystem>().Stop();
                    
                }
            }          
        }
        else
        {
            StartCoroutine(RemoveBullets());
            timer = 0;
        }
    }

    private IEnumerator RemoveBullets()
    {
        yield return new WaitForSeconds(1f);
        Destroy(bullets[0]);
        bullets.RemoveAt(0);
    }

    private void Start ()
    {   
        towerTransform = transform;

        towerRendererList = new List<Renderer>();
        bullets = new List<GameObject>();
        bulletsToRemove = new List<GameObject>();

        for (int i = 0; i < GetComponentsInChildren<Renderer>().Length; i++)
        {
            towerRendererList.Add(GetComponentsInChildren<Renderer>()[i]);
        }      

        towerRangeTransform = transform.GetChild(1);

        rangeCollider = towerRangeTransform.gameObject.GetComponent<RangeCollider>();

        var randomNumber = Random.Range(5, 30);

        towerRangeTransform.localScale = new Vector3(randomNumber, 0.0001f, randomNumber);

	}

    private void Update()
    {

        if (!IsTowerBuilded && GameManager.Instance.UISystem.IsBuildModeActive)
        {
            StartTowerBuild();
        }
        else
        {
            IsTowerBuilded = EndTowerBuild();
        }
        
        if (IsTowerBuilded)
        {
            if (rangeCollider.IsCreepInRange)
            {
                RotateTowerAtCreep();
                ShootAtCreep(40f);
            }
            else
            {
                RotateTowerToDefault();
            }
        }
        
       
    }
    
}
