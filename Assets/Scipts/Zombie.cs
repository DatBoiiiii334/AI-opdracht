using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour {

    private NavMeshAgent myAgent;
    public Transform player;

	// Use this for initialization
	void Start () {
        myAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        myAgent.SetDestination(player.position);
	}
}
