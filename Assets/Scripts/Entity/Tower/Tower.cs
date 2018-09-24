using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject Bullet;
    public bool IsTowerBuilded;

    private Transform towerRangeTransform, towerTransform;
    private List<Renderer> towerRendererList;
    private List<GameObject> bulletList;
    private List<ParticleSystem> bulletParticleSystemList;
    private RangeCollider rangeCollider;
    private float timer;
 

    private float CalcDistance(Vector3 position1, Vector3 position2)
    {
        float xD = position2.x - position1.x;
        float yD = position2.y - position1.y; 
        float zD = position2.z - position1.z;
        return xD * xD + yD * yD + zD * zD;
    }
    
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
                bulletList.Add(Instantiate(Bullet, towerTransform.position, Quaternion.Euler(0, 0, 0)));
                
                bulletParticleSystemList.Add(bulletList[bulletList.Count - 1].GetComponent<ParticleSystem>());              
            }
          
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (Vector3.Distance(bulletList[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position) > 10)
                {
                    bulletList[i].transform.position = Vector3.Lerp(bulletList[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position, Time.deltaTime * 10f);
                }
                else
                {
                    var em = bulletParticleSystemList[i].emission;
                    em.enabled = false;                   
                }
            }          
        }
        else
        {
            StartCoroutine(RemoveBullets(bulletParticleSystemList[0].main.startLifetime.constant));
            timer = 0;
        }
    }

    private IEnumerator RemoveBullets(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bulletList.Count > 0)
        {
            Destroy(bulletList[0]);
            bulletList.RemoveAt(0);
            bulletParticleSystemList.RemoveAt(0);
        }
    }

    private void Start ()
    {   
        towerTransform = transform;

        towerRendererList = new List<Renderer>();
        bulletList = new List<GameObject>();
        bulletParticleSystemList = new List<ParticleSystem>();

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
