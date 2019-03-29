using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Chicken : MonoBehaviour
{
    private static PlayerMovement3D _player;
    //States in statemachine
    public enum State { Search, Aggro, Flee, Rest };
    public State myState;
    public bool Close;

    public float TargetRange;
    public Transform target;
    public Vector3 OldPos;
    public Vector3 targetOldPosition;
    public Vector3 targetOldPosition2;
    public Transform RespawnPoint;
    public float speed;
    public float normalSpeed;
    public float aggroSpeed;
    public bool IsResting;


    //Explode related
    public GameObject exploEffect;
    public float timer = 3f;
    public float countdown;
    public bool hasExploded = false;
    public float radius = 4f;
    public float force = 700;

    //Waypoints
    public GameObject[] WayPoints;
    public int currentWP = 0;

    Vector3[] path;
    int targetIndex;


    //Player detecing related variables
    public float SightRange;
    public bool playerSight;



    void Start()
    {

        _player = GetComponent<PlayerMovement3D>();
        //Speed related 
        speed = 3f;
        normalSpeed = speed;
        aggroSpeed = 8f;
        countdown = timer;

        //Stamina related

        SightRange = 4f;


        currentWP = Random.Range(0, WayPoints.Length);
        targetOldPosition = target.position;
        myState = State.Search;

    }

    void Update()
    {
        
        if (hasExploded) {
            countdown -= Time.deltaTime;
            if(countdown <= 0) {
                Explode();

            }
        }

        //The actual switch state machine
        switch (myState) {
            case State.Aggro:
                Follow();
                break;

            case State.Search:
                Search();
                break;
        }


        //Calculating the distance between the Unit and its targeted waypoint
        var distance = Vector3.Distance(transform.position, WayPoints[currentWP].transform.position);

        //if the unit gets within a radius smaller than 1 of the targeted waypoint, then he gets a new waypoint assigned at random
        if (distance < 1f) {
            currentWP = Random.Range(0, WayPoints.Length);
        }

        if (Vector3.Distance(transform.position, target.position) < SightRange) {
            myState = State.Aggro;

            //Debug.Log("Aggro");
        }
        else {
            //Debug.Log("Searching");
            myState = State.Search;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void Explode()
    {
        Instantiate(exploEffect, transform.position, transform.rotation);
        hasExploded = false;
        //show effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in colliders) {
            if(nearbyObject.gameObject.tag == "player") {
                _player.lives -= 1;
                
                Debug.Log("hit player");
            }
        }
        
    }


    public void Follow()
    {
        Close = false;

        if (Vector3.Distance(targetOldPosition, target.position) > TargetRange) {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            targetOldPosition = target.position;
            //MyPos = transform.position;
        }

        if (Vector3.Distance(transform.position, target.position) < 1.5f) {
            hasExploded = true;
            Debug.Log("Explode");
        }
        //Enhanced speed
        //speed = aggroSpeed;

    }

    public void Search()
    {
        if (Close) {
            myState = State.Aggro;
        }


        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(direction);
        transform.rotation = look;

        if (WayPoints.Length > 0) {
            speed = normalSpeed;
            CheckForTagets();
            if (Vector3.Distance(targetOldPosition, WayPoints[currentWP].transform.position) > 0) {
                PathRequestManager.RequestPath(transform.position, WayPoints[currentWP].transform.position, OnPathFound);
                targetOldPosition = WayPoints[currentWP].transform.position;
            }
        }
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") {
            if (IsResting) {
                myState = State.Flee;
            }
        }
    }

    public void CheckForTagets()
    {
        transform.LookAt(targetOldPosition);


        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, SightRange)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.cyan);
            //enumState = State.Aggro;
            if (hit.transform.tag == "Player") {

            }
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful) {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        targetIndex = 0;
        while (true) {
            if (transform.position == currentWaypoint) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            //Move towards your target at your current speed times time
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}