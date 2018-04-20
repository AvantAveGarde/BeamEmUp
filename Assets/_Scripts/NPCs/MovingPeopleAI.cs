using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovingPeopleAI : MonoBehaviour {
    public float movementSpeed;
    public float rotateSpeed;
    public float waitTime;
    public Animator AnimController;
    public ItemData itemData;

    public float waitTimeCounter;
    private float switchProbability = 4f;

    private bool personWaiting;
    private bool personMovingForwards;
    private bool personMoving;

    [SerializeField]
    private int wayPointIndex = 0;

    public List<WayPoint> wayPoints;

    [SerializeField]
    Transform destination;

    NavMeshAgent navMeshAgent;



    // Use this for initialization
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = movementSpeed;
        navMeshAgent.angularSpeed = rotateSpeed;
        if (wayPoints.Count >= 2)
        {
            personMovingForwards = true;
            SetDestination();
            ChangePatrolPoint();
        }
        else
        {
            print("Insufficient Amount of Waypoints on " + gameObject.name);
        }

    }

    public void Zap()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        UpdateAnimContoller(personMoving);
        if (personMoving && navMeshAgent.remainingDistance <= 1f)
        {
            personMoving = false;
            personWaiting = true;
            waitTimeCounter = waitTime;
   
        }

        if (personWaiting)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                personWaiting = false;
                SetDestination();
                ChangePatrolPoint();
            }
        }

    }

    private void UpdateAnimContoller(bool Moving)
    {
        if (Moving)
        {
            var NormalizedSpeed = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
            //Debug.Log(navMeshAgent.velocity.magnitude + " NavMeshCUrrent Speed :" + navMeshAgent.speed);
            AnimController.SetFloat("MoveSpeed", NormalizedSpeed);
        }
        else
           AnimController.SetFloat("MoveSpeed", 0.0f);
    }

    void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(wayPoints[wayPointIndex].transform.position);
            personMoving = true;
        }
    }

    void ChangePatrolPoint()
    {
        if (Random.Range(0f, 11f) < switchProbability)
        {
            personMovingForwards = !personMovingForwards;
        }
        if (personMovingForwards)
        {
            wayPointIndex = (wayPointIndex + 1) % wayPoints.Count;
        }
        else
        {
            if (wayPointIndex > 0)
            {
                wayPointIndex = Random.Range(0, wayPoints.Count);
            }
        }
    }
}
