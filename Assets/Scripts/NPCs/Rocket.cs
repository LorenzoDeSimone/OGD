using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class Rocket : NetworkBehaviour
{
    public GameObject target;
    [Range(0.0f, 1.0f)]
    public float steeringMultiplier = 0.1f;
    public float speed = 10.0f;

    private Vector2 targetDirection;
    private Rigidbody2D myRigidBody;
    private Transform myTransform;

    private GameObject playerWhoShotMe;

    public GameObject GetPlayerWhoShot()
    {
        return playerWhoShotMe;
    }

    public void SetPlayerWhoShot(GameObject player)
    {
        playerWhoShotMe = player;
    }

    // Use this for initialization
    void Start ()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();

        Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.y);
        targetDirection = (targetPosition - myRigidBody.position).normalized;
        myTransform.right = targetDirection;
    }

    // Update is called once per frame
    void Update ()
    {
        float actualSteeringMultiplier = speed * steeringMultiplier;
   
        //currentDirection = (target.transform.position - myTransform.position).normalized;
        //myTransform.position = new Vector2(myTransform.position.x, myTransform.position.y) + currentDirection * speed;
        Vector2 currentDirection = myTransform.right;
        Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.y);

        targetDirection = (targetPosition - myRigidBody.position).normalized;
        Debug.DrawRay(myRigidBody.position, currentDirection * speed, Color.green);//Pre Steering velocity
        Debug.DrawRay(myRigidBody.position, targetDirection * speed, Color.gray);//Desired velocity

        Vector2 steeringVector = (targetDirection - currentDirection) * actualSteeringMultiplier;
        Debug.DrawRay(new Vector2(myRigidBody.position.x, myRigidBody.position.y) + currentDirection * speed, steeringVector, Color.red);

        //currentDirection = currentDirection + steeringVector;

        Debug.DrawRay(myRigidBody.position, (currentDirection * speed + steeringVector) * Time.deltaTime, Color.yellow);//Post Steering velocity

        myRigidBody.position = (new Vector2(myRigidBody.position.x, myRigidBody.position.y) + (currentDirection * speed + steeringVector)*Time.deltaTime);
        myTransform.right = (currentDirection * speed + steeringVector).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        MobilePlayerController newTargetPlayer = collider.GetComponent<MobilePlayerController>();
        Target target = collider.GetComponent<Target>();
        GravityField gravityField = collider.GetComponent<GravityField>();

        //What to do if collider is a target
        if (target != null)
        {
            //Debug.Log("Target Hit!");
            NetworkServer.UnSpawn(gameObject);
            gameObject.SetActive(false);
        }

        //What to do if collider is a gravity field
        if (gravityField !=null)
        {
            //Debug.Log("Platform Hit!");
            NetworkServer.UnSpawn(gameObject);
            gameObject.SetActive(false);
        }

        //What to do if collider is a player
        if (newTargetPlayer != null)
        { 
            if (newTargetPlayer.gameObject.Equals(playerWhoShotMe))
            {
                //Debug.Log("My rocket can do me no harm!");
            }
            else
            {
                //Debug.Log("Other Player Hit!");
                //NetworkServer.UnSpawn(gameObject);
                //gameObject.SetActive(false);
                //Insert methods for losing coins
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        MobilePlayerController newTargetPlayer = collider.GetComponent<MobilePlayerController>();
        if (newTargetPlayer != null && newTargetPlayer.gameObject.Equals(playerWhoShotMe))
            playerWhoShotMe = null;//The rocket forgets who shot it and it can hit him too after it exits first time from the shooter player trigger
    }
}
