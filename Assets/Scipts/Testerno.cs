using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testerno : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    public GameObject Theplayer;
    private Transform playerPos;
    private Transform MyPos;
 
    // Start is called before the first frame update
    void Start()
    {
        playerPos = Theplayer.transform;
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, playerPos.position) > stoppingDistance){
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed);
        }
    }

}
