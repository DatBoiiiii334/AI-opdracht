using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject Player;
    public Transform target;
	private Vector3 TargetPos;


	public float movespeed;
    public float ZoomSpeed;


    // Update is called once per frame
    void Update () {
		TargetPos = new Vector3 (Player.transform.position.x , Player.transform.position.y + 10, Player.transform.position.z -10f);
		transform.position = Vector3.Lerp (transform.position,TargetPos,movespeed * Time.deltaTime);


        ZoomIn();   //Do this function
    }

    void ZoomIn()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            GetComponent<Camera>().fieldOfView = 30f; 
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 ) {
            GetComponent<Camera>().fieldOfView = 60f;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            GetComponent<Camera>().fieldOfView = 90f;
        }
    }
}
