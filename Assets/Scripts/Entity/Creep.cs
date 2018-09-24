using UnityEngine;


public class Creep : MonoBehaviour
{
    public bool ReachedLastWaypoint;

    private Transform creepTransform;
    private float speed;
    private bool waypointReached;
    private int waypointIndex;

    private void Start ()
    {
        GameManager.Instance.CreepList.Add(gameObject);

        speed = 200f;

        creepTransform = transform;
        creepTransform.position = GameManager.Instance.CreepSpawnPoint.transform.position;
    }

    private void Update()
    {
        waypointReached = GameManager.CalcDistance(creepTransform.position, GameManager.Instance.WaypointList[waypointIndex].transform.position) < 50;

        if (waypointIndex < GameManager.Instance.WaypointList.Count - 1)
        {
            if (!waypointReached)
            {
                creepTransform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);

                var lookRotation = Quaternion.LookRotation(GameManager.Instance.WaypointList[waypointIndex].transform.position - creepTransform.position);
                creepTransform.rotation = Quaternion.Lerp(creepTransform.rotation, lookRotation, Time.deltaTime * 10f);
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
