﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class Missile : NetworkBehaviour
{
    public GameObject target;
    [Range(0.0f, 1.0f)]
    public float steeringMultiplier = 0.1f;
    public float speed = 10.0f;

    private Vector2 targetDirection;
    private Rigidbody2D myRigidBody;
    private Transform myTransform;

    void OnEnable ()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();

        if (target != null)
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.y);
            targetDirection = (targetPosition - myRigidBody.position).normalized;
            myTransform.right = targetDirection;
        }
        else
        {
            Debug.Log("OnEnable ->T null");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (target != null)
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

            myRigidBody.position = (new Vector2(myRigidBody.position.x, myRigidBody.position.y) + (currentDirection * speed + steeringVector) * Time.deltaTime);
            myTransform.right = (currentDirection * speed + steeringVector).normalized; 
        }
        else
            Debug.Log("Update -> T null");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        MobilePlayerController newTargetPlayer = collider.GetComponent<MobilePlayerController>();
        Target target = collider.GetComponent<Target>();
        GravityField gravityField = collider.GetComponent<GravityField>();

        //What to do if collider is a target
        if (target != null)
        {
            if (newTargetPlayer != null)
            {
                
                    NetworkServer.UnSpawn(gameObject);
                    Destroy(gameObject);
            }
            else//Generic Target behaviour(just explodes without doing anything)
            {
                //Debug.LogError("Generic target Hit! " + target.gameObject.name);
                NetworkServer.UnSpawn(gameObject);
                Destroy(gameObject);
            }
        }

        //What to do if collider is a gravity field
        if (gravityField !=null)
        {
            //Debug.LogError("Platform Hit!");
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //MobilePlayerController newTargetPlayer = collider.GetComponent<MobilePlayerController>();
        //if (newTargetPlayer != null && newTargetPlayer.gameObject.Equals(playerIDWhoShotMe))
        //    playerIDWhoShotMe = null;//The rocket forgets who shot it and it can hit him too after it exits first time from the shooter player trigger
    }
}
