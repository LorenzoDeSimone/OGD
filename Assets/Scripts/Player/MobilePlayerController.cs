﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MobilePlayerController : NetworkBehaviour
    {
        public float speed = 1.0f;
        public float jumpPower = 100.0f;
        public float rotationSpeed = 5.0f;
        public float movementReduction = 10;
        public float EdgeCheckMultiplier = 1.1f;
        public float airResistance = 0.4f;

        private static readonly float rotationEpsilon = 0.999f;
        private bool freeFromJumpBlock = true;

        public float jumpControlStopWindow = 0.2f;

        private RaycastHit2D myGround;
        private GameObject nearestTarget;

        private Transform myTransform;
        private Rigidbody2D myRigidBody;

        private Vector2 groundCheck1, groundCheck2;

        private HashSet<GameObject> myGravityFields;//A collection of gravity fields currently in player's trigger
        private HashSet<GameObject> myTargets;//A collection of hittable targets currently in player's trigger

        private GravityField safeGravityField;//In case no GravityField is present in player's collider, this is used for attraction

        public enum MOVEMENT_DIRECTIONS { COUNTERCLOCKWISE, CLOCKWISE, STOP }

        private GameObject myTargetMarker;

        //Network prediction and interpolation variables
        private float lastSynchronizationTime = 0f;
        private float syncDelay = 0f;
        private float syncTime = 0f;

        private float timeStamp;

        private Vector3 syncStartPosition;

        [SyncVar]//(hook = "SyncEndPosition")]
        public Vector3 syncEndPosition;

        int i = 0;

        public int predictionIterations=3;

        private void SyncEndPosition(Vector3 newSyncEndPosition)
        {
            //syncTime = 0f;
            //syncDelay = Time.time - lastSynchronizationTime;
            //lastSynchronizationTime = Time.time;
            //syncStartPosition = myTransform.position;
            Debug.LogError("Bau!");
            RpcUpdatePosition(newSyncEndPosition);
             //myTransform.position = syncEndPosition = newSyncEndPosition;
            //Debug.Log("SyncDelay " + syncDelay);
            //Debug.LogError("ARRRRRR, HOOK!    New sync end position: "+ newSyncEndPosition);
        }

        [ClientRpc]
        private void RpcUpdatePosition(Vector3 position)
        {
            myTransform.position = position;
        }

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();

            syncEndPosition = myTransform.position;
            syncStartPosition = myTransform.position;

            myGravityFields = new HashSet<GameObject>();
            myTargets = new HashSet<GameObject>();
            myTargetMarker = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Target Marker"));

            myGround = GetMyGround();
            nearestTarget = GetNearestTargetAndMarkIt();

            groundCheck1 = myTransform.Find("Ground Check 1").position;
            groundCheck2 = myTransform.Find("Ground Check 2").position;

            timeStamp = 0;

            /*if (isLocalPlayer)
            {
                Debug.Log("local player");
                StartCoroutine(SendNetworkPositionLoop());
            }*/
        }

        void Update()
        {
            groundCheck1 = myTransform.Find("Ground Check 1").position;
            groundCheck2 = myTransform.Find("Ground Check 2").position;
            //Debug.Log(syncTime / syncDelay);

            myTargetMarker.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;//REMOVE FROM UPDATE ASAP
            myGround = GetMyGround();
            nearestTarget = GetNearestTargetAndMarkIt();

            if (isLocalPlayer)
            {
                LocalMovement();
                CmdSendServerMyPosition(myTransform.position);
            }
            else
            {
                SyncedMovement();
            }

            ApplyRotation(false);
        }

        void FixedUpdate()
        {
            ApplyGravity();
        }

        [Command]
        private void CmdSendServerMyPosition(Vector3 position)
        {
            syncEndPosition = position;
        }

        private Vector2 predictNextPosition(int iterations, MOVEMENT_DIRECTIONS movementDirection)
        {
            Vector2 currPosition = myTransform.position;
            for (int i = 1; i <= iterations; i++)
            {
                currPosition = Move(currPosition, movementDirection, Time.deltaTime);
            }
            return currPosition;
        }

        private void LocalMovement()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                myTransform.position = Move(myTransform.position, MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE, Time.deltaTime);
                //syncEndPosition = predictNextPosition(predictionIterations, MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE);//Move(myTransform.position, MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE, Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                myTransform.position = Move(myTransform.position, MOVEMENT_DIRECTIONS.CLOCKWISE, Time.deltaTime);
                //syncEndPosition = predictNextPosition(predictionIterations, MOVEMENT_DIRECTIONS.CLOCKWISE);//Move(myTransform.position, MOVEMENT_DIRECTIONS.CLOCKWISE, Time.deltaTime);
            }
            //else
                //syncEndPosition = myTransform.position;

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        }

        private void SyncedMovement()
        {
            //syncTime += Time.deltaTime;
            //Debug.LogError("syncTime: " + syncTime + "||syncDelay: " + syncDelay + "||syncTime/syncDelay " + syncTime / syncDelay);
            //myTransform.position = Vector3.Slerp(syncStartPosition, syncEndPosition, syncTime/syncDelay);
            myTransform.position = syncEndPosition;
            //Debug.LogError("syncStart: " + syncStartPosition + "|| syncEnd: "+syncEndPosition);
        }

        private void ApplyGravity()
        {
            //float distance = Vector2.Distance(myGround.point, myTransform.position);
            Vector2 gravityVersor;
            GravityField myGravityField = myGround.collider.GetComponent<GravityField>();

            if (Vector2.Distance(myTransform.position, myGround.point) > GetCharacterCircleCollider2D().radius * 10f)
                gravityVersor = (myGravityField.gameObject.transform.position - myTransform.position).normalized;
            else
                gravityVersor = -myGround.normal;

            Debug.DrawRay(myTransform.position, gravityVersor, Color.red);
            GetComponent<Rigidbody2D>().AddForce(gravityVersor * myGravityField.mass);///distance);
        }

        private Vector3 GetMeanVector(Vector2[] positions)
        {
            if (positions.Length == 0)
                return Vector2.zero;
            float x = 0f;
            float y = 0f;
            foreach (Vector2 pos in positions)
            {
                x += pos.x;
                y += pos.y;
            }
            return new Vector2(x / positions.Length, y / positions.Length);
        }

        private void ApplyRotation(bool forceTargetRotation)
        {
            //Forward -> blue arrow in the editor
            //Normal -> Normal of current gravity field
            //We calculate the quaternion rotation that has the same forward vector of the current ground
            //but has the ground mean normal as the upward vector
            RaycastHit2D myGround = GetMyGround();
            Quaternion targetRotation = Quaternion.LookRotation(myGround.transform.forward, myGround.normal);
            //Debug.Log(Quaternion.Dot(transform.rotation, targetRotation));

            //If the rotation to do is very small, we just apply it directly
            if (forceTargetRotation || Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) > rotationEpsilon)
                transform.rotation = targetRotation;
            else//else, we interpolate to make the rotation smooth
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private RaycastHit2D GetMyGround()
        {
            float candidateMinDistance = float.MaxValue;
            //Finds first ground with a raycast under himself (Guaranteed to be found FIRST TIME ONLY by level design!)
            RaycastHit2D candidateNearestGround = Physics2D.Raycast(myTransform.position,
                                                 -myTransform.up,
                                                  Mathf.Infinity,
                                                  LayerMask.GetMask("Walkable"));
            if (safeGravityField == null)
                return candidateNearestGround;
            else if (myGravityFields.Count == 0)
            {
                //Debug.Log("0 gravityFields");
                return Physics2D.Raycast(myTransform.position,
                                         safeGravityField.transform.position - myTransform.position,
                                         Mathf.Infinity,
                                         LayerMask.GetMask("Walkable"));
            }
            else
            {
                //Debug.Log("At least one gravity field");
                foreach (GameObject currField in myGravityFields)
                {
                    RaycastHit2D currRaycastHit2D = Physics2D.Raycast(myTransform.position,
                                                                      currField.transform.position - myTransform.position,
                                                                      Mathf.Infinity,
                                                                      LayerMask.GetMask("Walkable"));

                    float currDistance = Vector2.Distance(myTransform.position, currRaycastHit2D.point);

                    if (currDistance < candidateMinDistance)
                    {
                        candidateNearestGround = currRaycastHit2D;
                        candidateMinDistance = currDistance;
                    }
                }
                return candidateNearestGround;
            }
        }

        public bool IsGrounded()
        {
            //float myCircleCollider2DRadius = getCharacterCircleCollider2D().GetComponent<CircleCollider2D>().radius;
            //return Physics2D.Raycast(myTransform.position - myTransform.up * myCircleCollider2DRadius, -GetSmoothedNormal(), 2f, LayerMask.GetMask("Walkable"));
            return Physics2D.OverlapArea(groundCheck1, groundCheck2, LayerMask.GetMask("Walkable"));
        }

        public bool CanShoot()
        {
            return true;//Placeholder before rocket count implementation
        }


        //Movement routines called by the input manager
        public Vector3 Move(Vector2 startPosition, MOVEMENT_DIRECTIONS movementDirection, float deltaTime)
        {
            //if (!CanMove())
            //return myTransform.position;

            GravityField myGravityField = myGround.collider.GetComponent<GravityField>();
            RaycastHit2D platformEdge;

            Vector2 movementVersor, movementPerpendicularDown, whereGroundShouldBe, recalculatedNextPlayerPoint;

            if (movementDirection.Equals(MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE))
            {
                GetComponent<SpriteRenderer>().flipX = true;//TEMP WORKAROUND, Animations will be implemented
                movementVersor = new Vector3(-myGround.normal.y, myGround.normal.x);
                movementPerpendicularDown = -myGround.normal;//new Vector2(-movementVersor.y, movementVersor.x).normalized;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;//TEMP WORKAROUND, Animations will be implemented
                movementVersor = new Vector3(myGround.normal.y, -myGround.normal.x);
                movementPerpendicularDown = -myGround.normal;// new Vector2(movementVersor.y, -movementVersor.x).normalized;
            }

            Vector2 nextPlayerPoint = new Vector2(startPosition.x, startPosition.y) + movementVersor * speed * 0.2f;
            Vector2 myPosition = new Vector2(startPosition.x, startPosition.y);
            Vector2 BackRaycastDirection = -movementVersor;//(myGravityField.transform.position - myTransform.position).normalized;



            //Edge detection code

            //Casts a ray with the direction of the antinormal of the playne starting from the next predicted player position to see if there will be ground
            RaycastHit2D nextGroundCheck = Physics2D.Raycast(nextPlayerPoint, movementPerpendicularDown,
                                                               GetCharacterCircleCollider2D().radius * EdgeCheckMultiplier,
                                                               LayerMask.GetMask("Walkable"));

            if (nextGroundCheck.collider == null && IsGrounded())//Edge detected: we obtain the next position on the platform that is grounded
            {
                /*Little Raycast scheme! P is player, arrows are raycasts, # is platform, N next player position corrected by the edge detection algorithm
                P--------->              N-P  the new corrected direction. Debug.DrawLine should help to understand what is going on =)
                ####<->N--|
                ####
                */
                whereGroundShouldBe = nextPlayerPoint + movementPerpendicularDown * GetCharacterCircleCollider2D().radius * EdgeCheckMultiplier;
                platformEdge = Physics2D.Raycast(whereGroundShouldBe, BackRaycastDirection, Mathf.Infinity, LayerMask.GetMask("Walkable"));
                if (platformEdge.collider != null && platformEdge.collider.gameObject.Equals(myGravityField.gameObject))
                {
                    //Debug.Log("Myland!");
                    recalculatedNextPlayerPoint = platformEdge.point + platformEdge.normal * GetCharacterCircleCollider2D().radius;
                    movementVersor = (recalculatedNextPlayerPoint - myPosition).normalized;

                    Debug.DrawLine(myTransform.position, nextPlayerPoint, Color.blue);
                    Debug.DrawLine(nextPlayerPoint, whereGroundShouldBe, Color.green);
                    Debug.DrawLine(whereGroundShouldBe, platformEdge.point, Color.yellow);
                    Debug.DrawLine(platformEdge.point, recalculatedNextPlayerPoint, Color.red);
                }
            }

            float distance = Vector2.Distance(myGround.point, myTransform.position);

            if (IsGrounded())//We apply movement vector directly is player is grounded
            {
                return startPosition + movementVersor * speed * deltaTime;
                //myRigidBody.AddForce(movementVersor * speed * Time.fixedDeltaTime);
                //myRigidBody.velocity = movementVersor * speed * Time.fixedDeltaTime;
                //myTransform.position = new Vector2(myTransform.position.x, myTransform.position.y) + movementVersor * speed * Time.fixedDeltaTime;
            }
            else//Otherwise, we decrease air control proportionally to his distance to the ground
            {
                return startPosition + movementVersor * speed * deltaTime;
                //myRigidBody.AddForce(movementVersor * speed * Time.fixedDeltaTime);
                //myRigidBody.position = new Vector2(myRigidBody.position.x, myRigidBody.position.y) + movementVersor * speed * 1 / Mathf.Pow(distance, airResistance) * Time.fixedDeltaTime;
            }
        }

        [Command]
        public void CmdShoot()
        {
            if (nearestTarget == null)
            {
                //Debug.Log("No targets in my area!");
                return;
            }

            GameObject rocket = (GameObject)Instantiate(Resources.Load("Prefabs/NPCs/Rocket"));

            rocket.transform.position = myTransform.position;
            rocket.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
            rocket.GetComponent<Rocket>().target = nearestTarget;
            rocket.GetComponent<Rocket>().SetPlayerWhoShot(gameObject);
            rocket.gameObject.SetActive(true);
            NetworkServer.Spawn(rocket);
        }

        public void Jump()
        {
            if (IsGrounded())
            {
                StartCoroutine(jumpControlStop());//We disable player movement input for a very small time in order to prevent some glitchy behaviour on critic situations
                                                  //might need a better solution for the future, works not so bad for now.
                RaycastHit2D myGround = GetMyGround();
                //ApplyRotation(true);
                GetComponent<Rigidbody2D>().AddForce(myGround.normal * jumpPower * Time.fixedDeltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            //Gravity Fields management
            GravityField newGravityField = collider.GetComponent<GravityField>();
            if (newGravityField != null)
            {
                myGravityFields.Add(newGravityField.gameObject);

                if (myGravityFields.Count == 1)
                    safeGravityField = newGravityField;
            }

            //Target management
            Target newTarget = collider.GetComponent<Target>();
            if (newTarget != null)
                myTargets.Add(newTarget.gameObject);

        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            //Gravity Fields management
            GravityField exitGravityField = collider.GetComponent<GravityField>();
            if (exitGravityField != null)
            {
                exitGravityField = collider.GetComponent<GravityField>();
                myGravityFields.Remove(exitGravityField.gameObject);

                if (myGravityFields.Count == 0)
                    safeGravityField = exitGravityField;

            }

            //Target management
            Target target = collider.GetComponent<Target>();
            if (target != null)
                myTargets.Remove(target.gameObject);

        }

        public CircleCollider2D GetCharacterCircleCollider2D()
        {
            CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();

            foreach (CircleCollider2D currCollider in colliders)
            {
                if (!currCollider.isTrigger)
                    return currCollider;
            }
            return null;
        }

        private GameObject GetNearestTargetAndMarkIt()
        {
            float candidateMinDistance = float.MaxValue;
            GameObject candidateNearestTarget = null;

            foreach (GameObject currTarget in myTargets)
            {
                float currDistance = Vector2.Distance(myTransform.position, currTarget.transform.position);

                if (currDistance < candidateMinDistance)
                {
                    candidateNearestTarget = currTarget.gameObject;
                    candidateMinDistance = currDistance;
                }
            }

            //Mark nearest target
            if (candidateNearestTarget != null)
            {
                myTargetMarker.gameObject.SetActive(true);
                myTargetMarker.transform.position = candidateNearestTarget.transform.position;
            }
            else
                myTargetMarker.gameObject.SetActive(false);

            return candidateNearestTarget;
        }

        private bool CanMove()
        {
            return freeFromJumpBlock;//Insert other booleans in && for other situations in which the player cannot move
        }

        IEnumerator<WaitForSeconds> jumpControlStop()
        {
            freeFromJumpBlock = false;
            //Debug.Log("disabling");
            yield return new WaitForSeconds(jumpControlStopWindow);
            //Debug.Log("enabling");
            freeFromJumpBlock = true;
        }

        IEnumerator<WaitForSeconds> SendNetworkPositionLoop()
        {
            Debug.Log("sENDING MY TRANSFORM");
            CmdSendServerMyPosition(myTransform.position);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
