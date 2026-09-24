using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Guard : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform[] patrolPoints;
    Transform currentPatrolPoint;
    int patrolPointIndex = 0;

    [SerializeField] float visionRadius;
    [SerializeField] LayerMask environmentLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatrolPoint = patrolPoints[0];

        agent.SetDestination(currentPatrolPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, currentPatrolPoint.position);

        if (distance < 1)
        {
            patrolPointIndex++;

            if (patrolPointIndex >= patrolPoints.Length)
            {
                patrolPointIndex = 0;
            }

            currentPatrolPoint = patrolPoints[patrolPointIndex];
            agent.SetDestination(currentPatrolPoint.position);

        }

    }

    public void HeardSomething(Collider thingWeHeard)
    { 
        Jeff jeff = thingWeHeard.GetComponent<Jeff>();
        if (jeff != null)
        {
            Debug.Log("Heard Jeff");
        }

    }
    
    public void SawSomething(Collider thingWeSaw)
    {

        Jeff jeff = thingWeSaw.GetComponent<Jeff>();
        if (jeff != null)
        {
            Vector3 guardFoward = transform.forward;
            guardFoward.y = 0;
            guardFoward.Normalize();

            Vector3 lineToJeff = (jeff.transform.position - transform.position).normalized;
            lineToJeff.y = 0;
            lineToJeff.Normalize();

            float dot = Vector3.Dot(guardFoward, lineToJeff);

            if (dot > visionRadius)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, lineToJeff, out hit, 1000, environmentLayer))
                {
                    Debug.Log("saw a wall");
                }
                else
                {
                    Debug.Log("Saw Jeff");
                }
            }

            else
            {
                Debug.Log("Did not see Jeff");
            }
        }
    }

}

