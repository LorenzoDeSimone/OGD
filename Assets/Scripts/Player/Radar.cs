using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private HashSet<GameObject> nearTargets;//A collection of hittable targets currently in player's radar

    private HashSet<GameObject> nearGravityFields;//A collection of gravity fields currently in player's radar
    private GravityField safeGravityField;//In case no gravity field is present in player's radar, this is used for attraction

    // Use this for initialization
    void Start ()
    {
        nearTargets = new HashSet<GameObject>();
        nearGravityFields = new HashSet<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Gravity Fields management
        GravityField gravityField = collider.GetComponent<GravityField>();
        Target target = collider.GetComponent<Target>();

        if (gravityField != null)
        {
            nearGravityFields.Add(gravityField.gameObject);

            if (nearGravityFields.Count == 1)
                safeGravityField = gravityField;
        }

        //Target management
        if (target != null)
            nearTargets.Add(target.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        GravityField gravityField = collider.GetComponent<GravityField>();
        Target target = collider.GetComponent<Target>();

        //Gravity Fields management
        if (gravityField != null)
        {
            nearGravityFields.Remove(gravityField.gameObject);

            if (nearGravityFields.Count == 0)
                safeGravityField = gravityField;
        }

        //Target management
        if (target != null)
            nearTargets.Remove(target.gameObject);
    }

    //Target methods
    public GameObject GetNearestTarget()
    {
        float candidateMinDistance = float.MaxValue;
        GameObject candidateNearestTarget = null;

        foreach (GameObject currTarget in nearTargets)
        {
            float currDistance = Vector2.Distance(transform.position, currTarget.transform.position);

            if (currDistance < candidateMinDistance)
            {
                candidateNearestTarget = currTarget.gameObject;
                candidateMinDistance = currDistance;
            }
        }
        return candidateNearestTarget;
    }

    //Gravity Fields methods

    //Finds nearest ground to the player
    public RaycastHit2D GetMyGround()
    {
        float candidateMinDistance = float.MaxValue;
        //Finds first ground with a raycast under himself (Guaranteed to be found FIRST TIME ONLY by level design!)
        RaycastHit2D candidateNearestGround = Physics2D.Raycast(transform.position,
                                             -transform.up,
                                              Mathf.Infinity,
                                              LayerMask.GetMask("Walkable"));
        if (safeGravityField == null)
            return candidateNearestGround;
        else if (nearGravityFields.Count == 0)
        {
            //Debug.Log("0 gravityFields");
            return Physics2D.Raycast(transform.position,
                                     safeGravityField.transform.position - transform.position,
                                     Mathf.Infinity,
                                     LayerMask.GetMask("Walkable"));
        }
        else
        {
            foreach (GameObject currField in nearGravityFields)
            {
                RaycastHit2D currRaycastHit2D = Physics2D.Raycast(transform.position,
                                                                  currField.transform.position - transform.position,
                                                                  Mathf.Infinity,
                                                                  LayerMask.GetMask("Walkable"));

                float currDistance = Vector2.Distance(transform.position, currRaycastHit2D.point);

                if (currDistance < candidateMinDistance)
                {
                    candidateNearestGround = currRaycastHit2D;
                    candidateMinDistance = currDistance;
                }
            }
            return candidateNearestGround;
        }
    }
}
