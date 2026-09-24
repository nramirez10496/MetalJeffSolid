using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public enum GuardStates
{
    PATROL,
    INVESTIGATE,
    PURSUE
}
public class Guard : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform[] patrolPoints;
    Transform currentPatrolPoint;
    int patrolPointIndex = 0;

    [SerializeField] float visionRadius;
    [SerializeField] LayerMask environmentLayer;

    GuardStates state = GuardStates.PATROL;
    Vector3 getPosition;
    [SerializeField] float investigateTime = 3f;
    float investigateTimer;
    Vector3 lastPosition;
    Jeff targetJeff;
    bool seesJeff = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatrolPoint = patrolPoints[0];
        agent.isStopped = false;
        agent.SetDestination(currentPatrolPoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GuardStates.PATROL:
                Patrol();
                break;
            case GuardStates.INVESTIGATE:
                Investigate();
                break;
            case GuardStates.PURSUE:
                Pursue();
                break;
        }
    }

    public void HeardSomething(Collider thingWeHeard)
    { 
        Jeff jeff = thingWeHeard.GetComponent<Jeff>();
        if (jeff != null)
        {
            //check wall
            Vector3 direction = jeff.transform.position - transform.position;
            float distance = direction.magnitude;
            direction.Normalize();

            RaycastHit hit;

            if(Physics.Raycast(transform.position, direction, out hit, distance, environmentLayer))
            {
                Debug.Log("Heard Jeff but wall");
                return;
            }

            Debug.Log("Heard Jeff");
            getPosition = jeff.transform.position;
            investigateTimer = investigateTime;

            ChangeState(GuardStates.INVESTIGATE);
            Debug.Log("INVESTIGATE");
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

            Vector3 lineToJeff = (jeff.transform.position - transform.position);
            lineToJeff.y = 0;
            lineToJeff.Normalize();
            float distance = lineToJeff.magnitude;

            float dot = Vector3.Dot(guardFoward, lineToJeff);

            if (dot > visionRadius)
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, lineToJeff, out hit, distance, environmentLayer))
                {
                    Debug.Log("saw a wall");
                    seesJeff = false;
                    return;
                }

                Debug.Log("Saw Jeff");
                targetJeff = jeff;
                lastPosition = jeff.transform.position;
                seesJeff = true;

                if(state == GuardStates.PATROL)
                {
                    getPosition = jeff.transform.position;
                    investigateTimer = investigateTime;
                    ChangeState(GuardStates.INVESTIGATE);
                    Debug.Log("INVESTIGATE");
                }
                else if (state == GuardStates.INVESTIGATE)
                {
                    ChangeState(GuardStates.PURSUE);
                    Debug.Log("PURSUE");
                }
                
            }

            else
            {
                Debug.Log("Did not see Jeff");
                seesJeff = false;
            }
        }
    }

    void Patrol()
    {
        agent.isStopped = false;

        if(!agent.hasPath)
        {
            agent.SetDestination(currentPatrolPoint.position);
        }

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
    void Investigate()
    {
        //stop, turn, investigate
        agent.isStopped = true;

        Vector3 direction = getPosition - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        investigateTimer-= Time.deltaTime;
        //wait 3 sec then patrol
        if(investigateTimer<=0)
        {
            agent.isStopped = false;
            agent.SetDestination(currentPatrolPoint.position);
            ChangeState(GuardStates.PATROL);
            Debug.Log("PATROL after pause");
        }
    }

    void Pursue()
    {
        agent.isStopped= false;

        if (targetJeff != null)
        {
            if (seesJeff)
            {
                //chase
                agent.SetDestination(targetJeff.transform.position);
                //remember position
                lastPosition = targetJeff.transform.position;
                //catch check
                float distance = Vector3.Distance(transform.position, targetJeff.transform.position);

                //catch jeff            
                if (distance < 1f)
                {
                    Debug.Log("CAUGHT");
                    GameOver();
                }
            }
            //lose jeff
            else
            {
                //go last seen
                agent.SetDestination(lastPosition);

                float distance = Vector3.Distance(transform.position, lastPosition);
                
                if (distance < 1f)
                {
                    getPosition = lastPosition;
                    investigateTimer = investigateTime;
                    agent.isStopped = true;

                    ChangeState(GuardStates.INVESTIGATE);
                    Debug.Log("INVESTIGATE");
                }
            }
        }
    }

    void ChangeState (GuardStates newState)
    {
        state = newState;
    }

    public void LostSight (Collider thingWeLost)
    {
        Jeff jeff = thingWeLost.GetComponent<Jeff>();
        if (jeff != null && jeff == targetJeff)
        {
            Debug.Log("Lost Jeff");
            seesJeff = false;
        }

    }

    void GameOver()
        SceneManager.LoadScene("GAME OVER");
    }

}


