using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ExtendedMonoBehaviour
{
    public ParticleSystem[] particleSystemList;

    protected override void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        for (int i = 0; i < particleSystemList.Length; i++)
        {
            var emissionModule = particleSystemList[i].emission;
            emissionModule.enabled = true;
            particleSystemList[i].Play();
        }
    }

    public void DisableParticles()
    {
        for (int i = 0; i < particleSystemList.Length; i++)
        {
            var emissionModule = particleSystemList[i].emission;
            emissionModule.enabled = false;
            particleSystemList[i].Stop();
        }
    }
}
