using UnityEngine;


public class CreepSample : MonoBehaviour
{
    public bool ReachedLastWaypoint;
    public CreepStats Stats;

    private Transform creepTransform;
    private float speed;
    private bool waypointReached;
    private int waypointIndex;

    private void Start ()
    {
        GameManager.Instance.CreepList.Add(gameObject);

        Stats = ScriptableObject.CreateInstance<CreepStats>();

        Stats.hp = 100f;
        Stats.entityName = "retard";
        Stats.armorIndex = 0;
        Stats.moveSpeed = 250f;
        
        speed = 200f;

        creepTransform = transform;
        creepTransform.position = GameManager.Instance.CreepSpawnPoint.transform.position + new Vector3(0, creepTransform.lossyScale.y, 0);
    }

    private void Update()
    {
        waypointReached = GameManager.CalcDistance(creepTransform.position, GameManager.Instance.WaypointList[waypointIndex].transform.position) < 70;

        if (waypointIndex < GameManager.Instance.WaypointList.Count - 1)
        {
            if (!waypointReached)
            {              
                creepTransform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
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
