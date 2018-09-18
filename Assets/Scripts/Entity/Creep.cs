using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creep : MonoBehaviour {

    public bool ReachedLastWaypoint;

    private Transform waypointTransform;
    private float speed;
    private NavMeshAgent agent;

    private void Start ()
    {
        speed = 5f;
        agent = GetComponent<NavMeshAgent>();

        waypointTransform = GameObject.Find("Waypoint6").transform;

       
        agent.SetDestination(waypointTransform.position);
        agent.Move(waypointTransform.position);
       
    }

  
}
