using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class OLDMobilePlayerController : NetworkBehaviour
    {
        public float speed = 1.0f;
        public float jumpPower = 100.0f;
        public float rotationSpeed = 5.0f;
        public float movementReduction = 10;
        public float EdgeCheckMultiplier = 1.1f;
        public float airResistance = 0.4f;
        public float timing = 0.01f;
        private float deltaPos;
        private static readonly float rotationEpsilon = 0.999f;
        private bool freeFromJumpBlock = true;

        public float jumpControlStopWindow = 0.2f;

        private RaycastHit2D myGround;
        private GameObject nearestTarget;
        private Radar myRadar;

        private Transform myTransform;
        private Rigidbody2D myRigidBody;

        private GameObject groundCheck1, groundCheck2;
        private GameObject missileStartPosition;

        public enum MOVEMENT_DIRECTIONS { COUNTERCLOCKWISE, CLOCKWISE, STOP }

        //Network prediction and interpolation variables
        private float lastSynchronizationTime = 0f;
        private float syncDelay = 0f;
        private float syncTime = 0f;
        private float timeStamp;

        private Vector3 syncStartPosition;
        public Vector3 syncEndPosition;

        private List<PlayerInput> InputHistorySentToServer, LocalInputHistory, InputBuffer; 

        public struct PlayerInput
        {
            public bool counterClockwise;
            public bool clockwise;
            public bool jump;
            public double timestamp;
        }

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
            myRadar = GetComponentInChildren<Radar>();

            syncEndPosition = myTransform.position;
            syncStartPosition = myTransform.position;

            myGround = myRadar.GetMyGround();

            groundCheck1 = myTransform.Find("Ground Check 1").gameObject;
            groundCheck2 = myTransform.Find("Ground Check 2").gameObject;

            timeStamp = 0;
            InputHistorySentToServer = new List<PlayerInput>();
            LocalInputHistory = new List<PlayerInput>();
            InputBuffer = new List<PlayerInput>();
            StartCoroutine(SendServerMyInputBuffer());
        }

        void Update()
        {
            //Debug.LogError("Velocity:" + myRigidBody.velocity);
            //Debug.Log(syncTime / syncDelay);
            myGround = myRadar.GetMyGround();

            if(!isLocalPlayer && !isServer)
            {
                SyncedMovement();
            }

            ApplyRotation(false);

            if (isServer)//Updates the client with input received
            {
                List<PlayerInput> History;

                //if (isLocalPlayer)
                //    History = LocalInputHistory;
                //else
                History = InputHistorySentToServer;
                foreach(PlayerInput currInput in History)
                {
                    myTransform.position = ExecuteInput(myTransform.position, currInput);//Server updates position on its machine
                }
                History.Clear();
                RpcSendPositionToClient(myTransform.position, Network.time);//Server updates position on all clients on a different machine than server
            }
        }

        [ClientRpc]
        private void RpcSendPositionToClient(Vector3 position, double timestampFromServer)
        {
            if (isLocalPlayer)
            {
                LinkedList<PlayerInput> LocalInputHistoryCopy = new LinkedList<PlayerInput>(LocalInputHistory);
                Vector2 newPosition = position;
                //bool FoundFirstNewerTimestamp = false;

                //Debug.Log("Time sent by server: " + Network.time);

                foreach (PlayerInput currInput in LocalInputHistoryCopy)
                {
                    //Debug.Log("CurrTimestamp: " + currInput.timestamp);
                    if (currInput.timestamp < timestampFromServer)//Older input to be discarded, it's done and it's ok
                        LocalInputHistory.Remove(currInput);
                    else//Input that is newer from last known server validated position (Not removed!)
                    {
                        /*if(!FoundFirstNewerTimestamp)
                        {
                            FoundFirstNewerTimestamp = true;
                            if (Vector2.Distance(position, currInput.position) < deltaPos)
                                return;
                        }*/
                        newPosition = ExecuteInput(newPosition, currInput);//From last validated position, we apply recent player input
                    }
                }
                myTransform.position = newPosition;
            }
            else
            {
                syncTime = 0f;
                syncDelay = Time.time - lastSynchronizationTime;
                lastSynchronizationTime = Time.time;
                syncStartPosition = myTransform.position;
                syncEndPosition = position;
            }
        }

        private Vector2 ExecuteInput(Vector3 startPosition, PlayerInput input)
        {
            Vector2 position = startPosition;

            if (input.counterClockwise)
                position = Move(position, MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE);
            else if (input.clockwise)
                position = Move(position, MOVEMENT_DIRECTIONS.CLOCKWISE);

            if (input.jump)
                Jump();

            return position;
        }

        void FixedUpdate()
        {
            ApplyGravity();
        }

        [Command]
        private void CmdSendServerMyInput(PlayerInput[] InputBuffer)
        {
            foreach(PlayerInput currInput in InputBuffer)
                InputHistorySentToServer.Add(currInput);
        }

        private Vector2 predictNextPosition(int iterations, MOVEMENT_DIRECTIONS movementDirection)
        {
            Vector2 currPosition = myTransform.position;
            for (int i = 1; i <= iterations; i++)
            {
                currPosition = Move(currPosition, movementDirection);
            }
            return currPosition;
        }

        public void LocalMoveandStoreInputInBuffer(PlayerInput input)
        {
            if (isLocalPlayer)
            {
                if (input.counterClockwise)
                {
                    if (!isServer)
                        myTransform.position = Move(myTransform.position, MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE);
                }
                else if (input.clockwise)
                {
                    if (!isServer)
                        myTransform.position = Move(myTransform.position, MOVEMENT_DIRECTIONS.CLOCKWISE);
                }
                if (input.jump)
                {
                    if (!isServer)
                        Jump();
                }

                //Safe programming redundant check: input is meaningless if every button is set to false (not pressed)
                //therefore should not be stored in the input buffer
                if (input.clockwise || input.counterClockwise || input.jump)
                {
                    if (!isServer)
                        LocalInputHistory.Add(input);
                    InputBuffer.Add(input);
                } 
            }
        }

        private void SyncedMovement()
        {
            syncTime += Time.deltaTime;
            Debug.Log("syncTime: " + syncTime + "||syncDelay: " + syncDelay + "||CLAMP01 syncTime/syncDelay " + Mathf.Clamp01(syncTime / syncDelay));
            myTransform.position = Vector3.Slerp(syncStartPosition, syncEndPosition, Mathf.Clamp01(syncTime / syncDelay));
            //myTransform.position = syncEndPosition;
            //Debug.LogError("syncStart: " + syncStartPosition + "|| syncEnd: "+syncEndPosition);
        }

        private void ApplyGravity()
        {
            //float distance = Vector2.Distance(myGround.point, myTransform.position);
            Vector2 gravityVersor;
            Platform myGravityField = myGround.collider.GetComponent<Platform>();

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
            Quaternion targetRotation = Quaternion.LookRotation(myGround.transform.forward, myGround.normal);
            //Debug.Log(Quaternion.Dot(transform.rotation, targetRotation));

            //If the rotation to do is very small, we just apply it directly
            if (forceTargetRotation || Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) > rotationEpsilon)
                transform.rotation = targetRotation;
            else//else, we interpolate to make the rotation smooth
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public bool IsGrounded()
        {
            //float myCircleCollider2DRadius = getCharacterCircleCollider2D().GetComponent<CircleCollider2D>().radius;
            //return Physics2D.Raycast(myTransform.position - myTransform.up * myCircleCollider2DRadius, -GetSmoothedNormal(), 2f, LayerMask.GetMask("Walkable"));
            return Physics2D.OverlapArea(groundCheck1.transform.position, groundCheck2.transform.position, LayerMask.GetMask("Walkable"));
        }

        //Movement routines called by the input manager
        public Vector3 Move(Vector2 startPosition, MOVEMENT_DIRECTIONS movementDirection)
        {
            //if (!CanMove())
            //return myTransform.position;

            Platform myGravityField = myGround.collider.GetComponent<Platform>();
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
                return startPosition + movementVersor * speed * Time.deltaTime;
                //myRigidBody.AddForce(movementVersor * speed * Time.fixedDeltaTime);
                //myRigidBody.velocity = movementVersor * speed * Time.fixedDeltaTime;
                //myTransform.position = new Vector2(myTransform.position.x, myTransform.position.y) + movementVersor * speed * Time.fixedDeltaTime;
            }
            else//Otherwise, we decrease air control proportionally to his distance to the ground
            {
                return startPosition + movementVersor * speed * Time.deltaTime;
                //myRigidBody.AddForce(movementVersor * speed * Time.fixedDeltaTime);
                //myRigidBody.position = new Vector2(myRigidBody.position.x, myRigidBody.position.y) + movementVersor * speed * 1 / Mathf.Pow(distance, airResistance) * Time.fixedDeltaTime;
            }
        }

        public void Jump()
        {
            if (IsGrounded())
            {
                StartCoroutine(jumpControlStop());//We disable player movement input for a very small time in order to prevent some glitchy behaviour on critic situations
                                                  //might need a better solution for the future, works not so bad for now.
                //ApplyRotation(true);
                GetComponent<Rigidbody2D>().AddForce(myGround.normal * jumpPower * Time.fixedDeltaTime);
            }
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

        IEnumerator<WaitForSeconds> SendServerMyInputBuffer()
        {
            while (true)
            {
                yield return new WaitForSeconds(timing);
                CmdSendServerMyInput(InputBuffer.ToArray());
                InputBuffer.Clear();
                //Debug.LogError("Input Sent to Server!");
            }
        }

    }
}
