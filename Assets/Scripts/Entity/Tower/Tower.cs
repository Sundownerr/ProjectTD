using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject Bullet;
    public bool IsTowerBuilded;

    public TowerStats towerStats;
    private Transform towerRangeTransform, towerTransform;
    private List<Renderer> towerRendererList;
    private List<GameObject> bulletList;
    private List<ParticleSystem> bulletParticleSystemList;
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

        towerTransform.rotation = Quaternion.Lerp(towerTransform.rotation, towerRotation, Time.deltaTime * 9f);
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
                if (GameManager.CalcDistance(bulletList[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position) > 30)
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
            if (bulletList.Count > 0)
            {
                StartCoroutine(RemoveBullets(bulletParticleSystemList[0].main.startLifetime.constant));
            }

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
            if (rangeCollider.CreepInRangeList.Count > 0 && rangeCollider.CreepInRangeList[0] != null && rangeCollider.IsCreepInRange)
            {
                RotateTowerAtCreep();
                ShootAtCreep(40f);
            }
            else
            {
                if(bulletList.Count > 0)
                {
                    Destroy(bulletList[0]);
                    bulletList.RemoveAt(0);
                    bulletParticleSystemList.RemoveAt(0);
                }

                RotateTowerToDefault();
            }
        }             
    }   
}
