using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;
    public Transform target;
    public GameObject targetObject;
    public float dotProductThreshold = -0.9f; // -1 is directly behind
    public float detectionDistance = 10f; // Note: Dot product does not know distance, this is needed
    public float onDetectionWaitTime = 3;
    public float turnedWaitTime = 5;
    public float rotationSpeed = 3f;
    public bool canHear;
    private Vector3 direction;
    private Quaternion targetRotation;
    private float distance;
    private float dot;
    private bool stopped = false;
    private bool canRotate = false;
    private StarterAssets.StarterAssetsInputs targetInput;

    int m_CurrentWaypointIndex;

    void Start()
    {
        targetInput = targetObject.GetComponent<StarterAssets.StarterAssetsInputs>();
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        // Dot product calculations
        direction = Vector3.Normalize(target.position - transform.position);
        distance = Vector3.Distance(transform.position, target.transform.position);
        dot = Vector3.Dot(direction, transform.forward);

        // Detect if something is close behind object within detectionDistance
        if (canHear && dot < dotProductThreshold && distance < detectionDistance && stopped == false && !targetInput.crouch)
        {
            stopped = true; // stopped boolean is set to true to ensure this only runs once until finished
            navMeshAgent.isStopped = true; // Interrupt AI movement, stop
            StartCoroutine(BehindYou()); // Run the turn around code as coroutine
        }
        if (canRotate == true) // Rotates 180 degrees if able to rotate, set by BehindYou()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
    }

    // On detection, wait onDetectionWaitTime seconds, turn around, then turnedWaitTime seconds later, resume
    IEnumerator BehindYou()
    {
        targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);

        yield return new WaitForSeconds(onDetectionWaitTime);
        canRotate = true; // This is necessary since Slerp only works in Update()
        yield return new WaitForSeconds(turnedWaitTime);

        canRotate = false;
        stopped = false; // Resume movement, reset detection
        navMeshAgent.isStopped = false;
    }
}
