using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerCameraController : MonoBehaviour
{

    public List<WaitPoint> points;
    public SpriteRenderer light;
    public GameObject background;

    private int currentPoint, i;

    [Range(0.0f, 1.0f)]
    float steeringMultiplier;
    float actualSteeringMultiplier;
    float speed, alpha;
    bool decrese;
    Vector3 currentDirection;
    Vector3 targetPosition;
    Vector3 targetDirection;
    Vector3 steeringVector;
    Vector3 nextPosition;

    void Start()
    {
        i = 0;
        alpha = 0;
        currentPoint = 0;
        decrese = false;
        currentDirection = (points[currentPoint].transform.position - transform.position).normalized;
        targetPosition = points[currentPoint].transform.position;
        speed = Mathf.Abs(Vector3.Distance(targetPosition, transform.position) / points[currentPoint].positionTime);
        steeringMultiplier = points[currentPoint].steeringMultiplier;
        currentDirection = ((points[currentPoint].transform.GetChild(0).position - transform.position) * speed + steeringVector).normalized;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            i++;
            currentPoint = i % points.Count;
            targetPosition = points[currentPoint].transform.position;
            speed = Mathf.Abs(Vector3.Distance(targetPosition, transform.position) / points[currentPoint].positionTime);
            steeringMultiplier = points[currentPoint].steeringMultiplier;
            currentDirection = ((points[currentPoint].transform.GetChild(0).position - transform.position) * speed + steeringVector).normalized;
        }
        /*
        if (i == 1 && transform.position == targetPosition)
        {
            if (decrese && alpha <= 0)
                decrese = false;
            else if (!decrese && alpha >= 20)
                decrese = true;

            if (decrese)
                alpha -= 0.8f;
            else
                alpha += 0.8f;
            light.color = new Color(1, 1, 1, alpha / 100);
        }
        else if (i == 2)
            light.color = new Color(1, 1, 1, 0);
        */
        actualSteeringMultiplier = speed * steeringMultiplier;
        targetDirection = (targetPosition - transform.position).normalized;

        Debug.DrawRay(transform.position, currentDirection * speed, Color.green);//Pre Steering velocity
        Debug.DrawRay(transform.position, targetDirection * speed, Color.gray);//Desired velocity

        steeringVector = (targetDirection - currentDirection) * actualSteeringMultiplier;

        Debug.DrawRay(transform.position + currentDirection * speed, steeringVector, Color.red);
        Debug.DrawRay(transform.position, (currentDirection * speed + steeringVector) * Time.deltaTime, Color.yellow);//Post Steering velocity

        nextPosition = transform.position + (currentDirection * speed + steeringVector) * Time.deltaTime;
        if (Vector3.Distance(nextPosition, transform.position) > Vector3.Distance(targetPosition, transform.position))
            nextPosition = targetPosition;
        if (i > 1)
            background.transform.position += nextPosition - transform.position;
        transform.position = nextPosition;
        currentDirection = (currentDirection * speed + steeringVector).normalized;
    }
}
