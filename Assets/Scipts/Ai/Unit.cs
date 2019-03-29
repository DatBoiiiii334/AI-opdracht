using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    //States in statemachine
    public enum State {Patrol, Aggro, Flee, Rest};
    public State enumState;
    public bool AllowAgro;

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

    //Waypoints
    public GameObject[] WayPoints;
    public int currentWP = 0;

    Vector3[] path;
    int targetIndex;

    //Stamina related variables
    public Slider StaminaBar;
    public float stamina = 5f;
    private float maxStamina;
    public float staminaTimer = 10f;

    //Player detecing related variables
    public float SightRange;
    public bool playerSight;

    //For changing of the material color
    public Color myColor = Color.black;
    public Renderer rend;


    void Start()
    {
        //Speed related 
        speed = 2.8f;
        normalSpeed = speed;
        aggroSpeed = 8f;

        //Stamina related
        maxStamina = stamina;

        SightRange = 5f;


        currentWP = Random.Range(0, WayPoints.Length);
        targetOldPosition = target.position;
        enumState = State.Patrol;

        //Stamina Bar
        StaminaBar.minValue = 0;
        StaminaBar.maxValue = maxStamina;
        StaminaBar.wholeNumbers = true;
        StaminaBar.value = stamina;

        //Its material color is now the color we want to give it
        rend.material.color = myColor;

    }

    void Update()
    {

        //The actual switch state machine
        switch (enumState) 
        {
            case State.Aggro:
                Aggro();
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Flee:
                Flee();
                break;

            case State.Rest:
                Rest();
                break;
        }

        //UI stamina bar slider is equal to the value of the unit's current stamina
        StaminaBar.value = stamina;

        //Calculating the distance between the Unit and its targeted waypoint
        var distance = Vector3.Distance(transform.position, WayPoints[currentWP].transform.position);

        //if the unit gets within a radius smaller than 1 of the targeted waypoint, then he gets a new waypoint assigned at random
        if (distance < 1f) {
            currentWP = Random.Range(0, WayPoints.Length);
        }
    }



    public void Aggro()
    {
        AllowAgro = false;
        IsResting = false;
        //Assign this color
        myColor.r = 300f;
        myColor.b = 0f;
        myColor.g = 0f;
        rend.material.color = myColor;

        if (Vector3.Distance(targetOldPosition, target.position) > TargetRange) {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                targetOldPosition = target.position; 
                //MyPos = transform.position;
            }
        //Enhanced speed
        speed = aggroSpeed;
        //speed += Time.deltaTime;
        //Drain stamina on aggro
        stamina -= Time.deltaTime;
        if (stamina <= 0) {
            enumState = State.Rest;
        }

    }

    public void Patrol()
    {
        if (AllowAgro) {
            enumState = State.Aggro;
        }

        IsResting = false;
        //Assign this color
        myColor.r = 0f;
        myColor.b = 30f;
        myColor.g = 0f;
        rend.material.color = myColor;

        //Regain stamina
        if (stamina < maxStamina) {
            stamina += Time.deltaTime;
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

    public void Rest()
    {
        IsResting = true;
        //Assign this color
        myColor.r = 0f;
        myColor.b = 0f;
        myColor.g = 300f;
        rend.material.color = myColor;

        speed = 0f;
        stamina += Time.deltaTime;
        if(stamina >= maxStamina) {
            stamina = maxStamina;
            enumState = State.Patrol;
        }
    }

    public void Flee()
    {
        IsResting = false;
        //Assign this color
        myColor.a = 0f;
        rend.material.color = myColor;
        speed = normalSpeed;

        if (stamina < maxStamina) {
            stamina += Time.deltaTime;
        }

        if (Vector3.Distance(targetOldPosition, RespawnPoint.transform.position) > 0) {
            PathRequestManager.RequestPath(transform.position, RespawnPoint.transform.position, OnPathFound);
            //targetOldPosition = RespawnPoint.transform.position;
            
        }

        if (Vector3.Distance(transform.position, RespawnPoint.transform.position) < 1f) {
            StartCoroutine(Respawning());
        }

    }

    IEnumerator Respawning()
    {
        yield return new WaitForSeconds(1f);
        myColor.r = 0f;
        yield return new WaitForSeconds(1f);
        myColor.r = 100f;

        enumState = State.Patrol;
        StopCoroutine(Respawning());
    }


    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player") {
            if (IsResting) {
                enumState = State.Flee;
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