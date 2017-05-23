using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class Missile : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float steeringMultiplier = 0.1f;
    public float speed = 10.0f;
    private GameObject target;
    private int targetId;
    private bool initDone = false;

    private void Start()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.GetComponent<PlayerDataHolder>().GetPlayerNetworkId() == targetId)
            {
                target = go;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float actualSteeringMultiplier = speed * steeringMultiplier;

        //currentDirection = (target.transform.position - myTransform.position).normalized;
        //myTransform.position = new Vector2(myTransform.position.x, myTransform.position.y) + currentDirection * speed;
        Vector3 currentDirection = transform.right;
        Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);

        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        Debug.DrawRay(transform.position, currentDirection * speed, Color.green);//Pre Steering velocity
        Debug.DrawRay(transform.position, targetDirection * speed, Color.gray);//Desired velocity

        Vector3 steeringVector = (targetDirection - currentDirection) * actualSteeringMultiplier;

        Debug.DrawRay(transform.position + currentDirection * speed, steeringVector, Color.red);
        Debug.DrawRay(transform.position, (currentDirection * speed + steeringVector) * Time.deltaTime, Color.yellow);//Post Steering velocity

        transform.position = transform.position + (currentDirection * speed + steeringVector) * Time.deltaTime;
        transform.right = (currentDirection * speed + steeringVector).normalized;
    }

    public void SetTargetId(int id)
    {
        targetId = id;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        MobilePlayerController newTargetPlayer = collider.GetComponent<MobilePlayerController>();
        GravityField gravityField = collider.GetComponent<GravityField>();

        if (newTargetPlayer != null)
        {
            Debug.LogError("Player Hit! " + target.gameObject.name);
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }

        //What to do if collider is a gravity field
        if (gravityField != null)
        {
            Debug.LogError("Platform Hit!");
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
