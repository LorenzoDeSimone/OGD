using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class Radar : MonoBehaviour
{
    private HashSet<GameObject> nearTargets;//A collection of hittable targets currently in player's radar

    private HashSet<GameObject> nearGravityFields;//A collection of gravity fields currently in player's radar
    private Platform safeGravityField;//In case no gravity field is present in player's radar, this is used for attraction

    public bool variableGravityField = true;
    public bool checkForGroundDown = true;

    // Use this for initialization
    void Start ()
    {
        nearTargets = new HashSet<GameObject>();
        nearGravityFields = new HashSet<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Platform gravityField = collider.GetComponent<Platform>();
        Target target = collider.GetComponent<Target>();

        if (variableGravityField || (!variableGravityField && nearGravityFields.Count == 0))
        {
            //Gravity Fields management

            if (gravityField != null)
            {
                nearGravityFields.Add(gravityField.gameObject);
                if (nearGravityFields.Count == 1)
                    safeGravityField = gravityField;
            } 
        }

        //Target management
        if (target != null)
            nearTargets.Add(target.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        Platform gravityField = collider.GetComponent<Platform>();
        Target target = collider.GetComponent<Target>();

        if (variableGravityField)
        {
            //Gravity Fields management
            if (gravityField != null)
            {
                nearGravityFields.Remove(gravityField.gameObject);

                if (nearGravityFields.Count == 0)
                    safeGravityField = gravityField;
            }
        }

        //Target management
        if (target != null)
            nearTargets.Remove(target.gameObject);
    }

    //Target methods
    public GameObject GetNearestTarget(Vector3 shootStartPosition)
    {
        float candidateMinDistance = float.MaxValue;
        GameObject candidateNearestTarget = null;

        foreach (GameObject currTarget in nearTargets)
        {
            float distanceFromShootingPoint = Vector2.Distance(shootStartPosition, currTarget.transform.position);

            if (distanceFromShootingPoint < candidateMinDistance)
            {
                //Check if shooting I would hit myself
                float distanceFromPlayer = Vector2.Distance(transform.position, currTarget.transform.position);
                if (distanceFromPlayer > distanceFromShootingPoint)
                {
                    //Checks if there is no walkable between the shooting position and the target
                    //Debug.DrawRay(shootStartPosition, currTarget.transform.position - shootStartPosition, Color.red);
                    RaycastHit2D currRaycastHit2D = Physics2D.Raycast(shootStartPosition,
                                                           currTarget.transform.position - shootStartPosition,
                                                           distanceFromShootingPoint,
                                                           LayerMask.GetMask("Walkable"));

                    if (currRaycastHit2D.collider == null)
                    {
                        candidateNearestTarget = currTarget.gameObject;
                        candidateMinDistance = distanceFromShootingPoint;
                    } 
                }
            }
        }
        return candidateNearestTarget;
    }

    //Gravity Fields methods

    //Finds nearest ground to the player
    public RaycastHit2D GetMyGround()
    {
        float candidateMinDistance = float.MaxValue;
        Vector2 myPosition = new Vector2(transform.position.x, transform.position.y);
        Collider2D movableCollider = GetComponentInParent<Movable>().GetComponent<Collider2D>();
        RaycastHit2D candidateNearestGround;

        if (checkForGroundDown)
        {
            //Finds first ground with a raycast under himself (Guaranteed to be found FIRST TIME ONLY by level design!)
            candidateNearestGround = Physics2D.Raycast(transform.position,
                                     -transform.up,
                                     Mathf.Infinity,
                                     LayerMask.GetMask("Walkable"));
        }
        else
        {
            //NONSENSE: just neeed for a RaycastHit2D that has no collider... I know... It's horrible =(
            candidateNearestGround = Physics2D.Raycast(transform.position,
                         transform.up,
                         0f,
                         LayerMask.GetMask("Water"));
        }

        if (safeGravityField == null)
            return candidateNearestGround;
        else if (nearGravityFields.Count == 0)
        {
            ColliderDistance2D distanceFromGravityField = safeGravityField.GetComponent<Collider2D>().Distance(movableCollider);

            //Debug.Log("0 gravityFields");
            return Physics2D.Raycast(transform.position,
                                     distanceFromGravityField.pointA - myPosition,
                                     Mathf.Infinity,
                                     LayerMask.GetMask("Walkable"));
        }
        else
        {
            foreach (GameObject currField in nearGravityFields)
            {
                ColliderDistance2D distanceFromGravityField = currField.GetComponent<Collider2D>().Distance(movableCollider);

                RaycastHit2D currRaycastHit2D = Physics2D.Raycast(transform.position,
                                                                  distanceFromGravityField.pointA - myPosition,
                                                                  Mathf.Infinity,
                                                                  LayerMask.GetMask("Walkable"));

                float currDistance = Vector2.Distance(transform.position, currRaycastHit2D.point);

                //Debug.DrawRay(myPosition, distanceFromGravityField.pointA - myPosition, Color.cyan);

                if (currDistance < candidateMinDistance)
                {
                    candidateNearestGround = currRaycastHit2D;
                    candidateMinDistance = currDistance;
                }
            }
            //if (candidateNearestGround.collider == null)
            //   Debug.LogError("Oh my!");

            return candidateNearestGround;
        }
    }
}

/*
//Gravity Fields methods
//Finds nearest ground to the player
public GravityFieldInfo GetMyGround()
{
   float candidateMinDistance;
   Collider2D movableCollider = GetComponentInParent<Movable>().GetComponent<Collider2D>();
   //Finds first ground with a raycast under himself (Guaranteed to be found FIRST TIME ONLY by level design!)
   GravityFieldInfo candidateGravityFieldInfo;

   ColliderDistance2D distanceFromGravityField = safeGravityField.GetComponent<Collider2D>().Distance(movableCollider);

   candidateGravityFieldInfo.normal = distanceFromGravityField.normal;
   candidateGravityFieldInfo.nearestPoint = distanceFromGravityField.pointA;
   candidateGravityFieldInfo.mass = safeGravityField.GetComponent<Platform>().mass;
   candidateMinDistance = Vector2.Distance(transform.position, safeGravityField.transform.position);

   foreach (GameObject currField in nearGravityFields)
   {
       Debug.DrawLine(transform.position, currField.transform.position, Color.green);
       float distanceBetweenCenters = Vector2.Distance(transform.position, currField.transform.position);
       if (distanceBetweenCenters < candidateMinDistance)
       {
           candidateMinDistance = distanceBetweenCenters;
           ColliderDistance2D currGravityFieldDistanceInfo = currField.GetComponent<Collider2D>().Distance(movableCollider);
           candidateGravityFieldInfo.normal = currGravityFieldDistanceInfo.normal;
           candidateGravityFieldInfo.nearestPoint = currGravityFieldDistanceInfo.pointA;
           candidateGravityFieldInfo.mass = currField.GetComponent<Platform>().mass;
       }
   }

   return candidateGravityFieldInfo;
}
}*/
