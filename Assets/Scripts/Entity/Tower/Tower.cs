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
    private List<Transform> bulletTransformList;
    private RangeCollider rangeCollider;
    private ObjectPool bulletPool;
    private int bulletCounter;

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
                bulletList.Add(bulletPool.GetObject());
                bulletTransformList.Add(bulletList[bulletList.Count - 1].transform);

                bulletList[bulletList.Count - 1].transform.position = towerTransform.position;
                bulletList[bulletList.Count - 1].transform.rotation = towerTransform.rotation;             

                bulletParticleSystemList.Add(bulletList[bulletList.Count - 1].GetComponent<ParticleSystem>());

                var em = bulletParticleSystemList[bulletParticleSystemList.Count - 1].emission;
                em.enabled = true;

                bulletList[bulletList.Count - 1].SetActive(true);

    
            }

            for (int i = 0; i < bulletList.Count; i++)
            {
                if (GameManager.CalcDistance(bulletList[i].transform.position, rangeCollider.CreepInRangeList[0].transform.position) > rangeCollider.CreepInRangeList[0].transform.lossyScale.x)
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
            bulletList[0].SetActive(false);
            bulletList.RemoveAt(0);
            bulletParticleSystemList.RemoveAt(0);
           // bulletTransformList.RemoveAt(0);
        }
    }

    private void Start ()
    {
        
        towerTransform = transform;

        towerRendererList = new List<Renderer>();
        bulletList = new List<GameObject>();
        bulletParticleSystemList = new List<ParticleSystem>();
        bulletTransformList = new List<Transform>();

        bulletPool = new ObjectPool
        {
            poolObject = Bullet
            //parent = towerTransform
        };

        bulletPool.Initialize();

        for (int i = 0; i < GetComponentsInChildren<Renderer>().Length; i++)
        {
            towerRendererList.Add(GetComponentsInChildren<Renderer>()[i]);
        }      

        towerRangeTransform = transform.GetChild(1);

        rangeCollider = towerRangeTransform.gameObject.GetComponent<RangeCollider>();

        var randomNumber = Random.Range(15, 30);

        towerRangeTransform.localScale = new Vector3(randomNumber, 0.0001f, randomNumber);

       StartCoroutine(UpdateTowerState());

    }

    private IEnumerator UpdateTowerState()
    {
        while (!IsTowerBuilded)
        {
            yield return new WaitForEndOfFrame();

            if (!IsTowerBuilded && GameManager.Instance.UISystem.IsBuildModeActive)
            {
                StartTowerBuild();
            }
            else
            {
                IsTowerBuilded = EndTowerBuild();

                StartCoroutine(LookForCreeps());
            }
        }              
    }

    private IEnumerator LookForCreeps()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (rangeCollider.CreepInRangeList.Count > 0 && rangeCollider.CreepInRangeList[0] != null && rangeCollider.IsCreepInRange)
            {
                RotateTowerAtCreep();
                ShootAtCreep(25f);
            }
            else
            {
                if (bulletList.Count > 0)
                {
                    for (int i = 0; i < bulletList.Count; i++)
                    {
                        bulletList[i].transform.Translate(Vector3.forward * 200, Space.Self);
                    }

                    StartCoroutine(RemoveBullets(0.2f));
                }

                RotateTowerToDefault();
            }
        }
    }
}


