using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject target;
    [Range(0.0f, 1.0f)]
    public float steeringMultiplier = 0.1f;
    public float speed = 10.0f;
    
    private Vector2 targetDirection;
    private Transform myTransform;

    // Use this for initialization
    void Start ()
    {
        myTransform = GetComponent<Transform>();
        targetDirection = (target.transform.position - myTransform.position).normalized;
        myTransform.right = targetDirection;
        steeringMultiplier = speed * steeringMultiplier;
    }

    // Update is called once per frame
    void Update ()
    {
        //currentDirection = (target.transform.position - myTransform.position).normalized;
        //myTransform.position = new Vector2(myTransform.position.x, myTransform.position.y) + currentDirection * speed;
        Vector2 currentDirection = myTransform.right;

        targetDirection = (target.transform.position - myTransform.position).normalized;
        Debug.DrawRay(myTransform.position, currentDirection * speed, Color.green);//Pre Steering velocity
        Debug.DrawRay(myTransform.position, targetDirection * speed, Color.gray);//Desired velocity

        Vector2 steeringVector = (targetDirection - currentDirection) * steeringMultiplier;
        Debug.DrawRay(new Vector2(myTransform.position.x, myTransform.position.y) + currentDirection * speed, steeringVector, Color.red);

        //currentDirection = currentDirection + steeringVector;

        Debug.DrawRay(myTransform.position, (currentDirection * speed + steeringVector) * Time.deltaTime, Color.yellow);//Post Steering velocity

        myTransform.position = (new Vector2(myTransform.position.x, myTransform.position.y) + (currentDirection * speed + steeringVector)*Time.deltaTime);
        myTransform.right = (currentDirection * speed + steeringVector).normalized;
    }

}
