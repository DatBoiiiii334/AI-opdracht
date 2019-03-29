using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement3D : MonoBehaviour
{
    private static Unit _unit;
    private Rigidbody myRigidbody;

    public float playerSpeed;
    public float lives;

    //respawn related
    public bool IsRespawning;
    public float reTimer;
    public float maxTimer;

    public int playerLifesMatter;
    public Text playerLifesText;
    public int Coins;
    public Text amountCoins;

    //Player color
    public Color playerColor;
    public Color ZombieColor;
    public Material SMatPlayer;

    // Start is called before the first frame update
    void Start()
    {
        _unit = FindObjectOfType<Unit>();
        myRigidbody = GetComponent<Rigidbody>();

        //Player stats
        playerSpeed = 5f;
        playerLifesMatter = 3;
        SMatPlayer.color = playerColor;

        maxTimer = 5f;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        playerLifesText.text = "Lives: " + playerLifesMatter;
        amountCoins.text = "Coins " + Coins;

        if (IsRespawning) {
            reTimer += Time.deltaTime;
            if(reTimer >= maxTimer) {
                IsRespawning = false;
                reTimer = 0;
            }
        }

        if(Coins == 10) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if(lives <= 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }
    
    }

    //Movement script DO NOT TOUCH
    void Movement() {

        if (Input.GetKey(KeyCode.A)) {  //Left
            myRigidbody.velocity = new Vector3(-playerSpeed, 0, 0);
        }

        if (Input.GetKey(KeyCode.D)) {  //Right
            myRigidbody.velocity = new Vector3(playerSpeed, 0, 0);
        }


        if (Input.GetKey(KeyCode.W)) {  //Up
            myRigidbody.velocity = new Vector3(0, 0, playerSpeed);
        }

        if (Input.GetKey(KeyCode.S)) {  //Down
            myRigidbody.velocity = new Vector3(0, 0, -playerSpeed);
        }
    }



    //Portal script 
    private void OnTriggerEnter(Collider collision)
    {
        //Going trouh Right portal 
        if (collision.gameObject.tag == "Portal1") {
            transform.position = new Vector3(14f, 0.22f, transform.position.z);
        }

        //Going trouh Left portal 2
        if (collision.gameObject.tag == "Portal2") {
            transform.position = new Vector3(-14f, 0.22f, transform.position.z);
        }

        if(collision.gameObject.tag == "Gem") {
            if(_unit.AllowAgro == false) {
                _unit.AllowAgro = true;
                Coins += 1;
                Destroy(collision.gameObject);
                Debug.Log("One down");
            }
        }

        if(collision.gameObject.tag == "Enemy") {
            if (!IsRespawning) {
                StartCoroutine(PlayerBlink());
                playerLifesMatter = playerLifesMatter - 1;
                IsRespawning = true;
            }
            
        }
        
    }


    IEnumerator PlayerBlink()
    {
        SMatPlayer.color = ZombieColor;

        yield return new WaitForSeconds(0.3f);
        SMatPlayer.color = playerColor;

        yield return new WaitForSeconds(0.3f);
        SMatPlayer.color = ZombieColor;

        yield return new WaitForSeconds(0.3f);
        SMatPlayer.color = playerColor;

        yield return new WaitForSeconds(0.3f);
        SMatPlayer.color = ZombieColor;

        yield return new WaitForSeconds(0.3f);
        SMatPlayer.color = playerColor;

        StopCoroutine(PlayerBlink());
    }


}
