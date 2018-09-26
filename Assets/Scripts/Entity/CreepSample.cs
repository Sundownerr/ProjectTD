using System.Collections;
using System;
using UnityEngine;
using Game.System;


namespace Game.Creep
{

    public class CreepSample : ExtendedMonoBehaviour
    {
        public bool ReachedLastWaypoint;
        public CreepStats Stats;

        private Transform creepTransform;
        private float speed;
        private bool waypointReached;
        private int waypointIndex;

        public float spawnEffectTime = 1;
        public AnimationCurve fadeIn;

        float timer = 0;
        Renderer _renderer;

        int shaderProperty;

        private void Start()
        {
            GameManager.Instance.CreepList.Add(gameObject);
            transform.parent = GameManager.Instance.CreepParent;

            Stats = ScriptableObject.CreateInstance<CreepStats>();

            Stats.hp = 100f;
            Stats.entityName = "retard";
            Stats.armorIndex = 0;
            Stats.moveSpeed = 450f;

            

            creepTransform = transform;
            creepTransform.position = GameManager.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);

            shaderProperty = Shader.PropertyToID("_cutoff");
            _renderer = GetComponent<Renderer>();

            StartCoroutine(ShowSpawnEffect());

        }

        private IEnumerator ShowSpawnEffect()
        {
            while (timer < spawnEffectTime)
            {
                timer += Time.deltaTime;

                _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));
                yield return new WaitForFixedUpdate();
            }


        }

        private void Update()
        {


            waypointReached = GameManager.CalcDistance(creepTransform.position, GameManager.Instance.WaypointList[waypointIndex].transform.position) < 70;

            if (waypointIndex < GameManager.Instance.WaypointList.Count - 1)
            {
                if (!waypointReached)
                {
                    creepTransform.Translate(Vector3.forward * Time.deltaTime * Stats.moveSpeed, Space.Self);
                    creepTransform.position = new Vector3(creepTransform.position.x, creepTransform.lossyScale.y, creepTransform.position.z);

                    var lookRotation = Quaternion.LookRotation(GameManager.Instance.WaypointList[waypointIndex].transform.position - creepTransform.position);

                    var rotation = Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);
                    rotation.z = 0;
                    rotation.x = 0;

                    creepTransform.localRotation = rotation;
                }
                else
                {
                    waypointIndex++;
                }
            }
            else
            {
                Destroy(gameObject, 1f);
                GameManager.Instance.CreepList.Remove(gameObject);
            }
        }
    }
}