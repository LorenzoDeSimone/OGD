using UnityEngine;
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

        private RaycastHit2D myGround;
        private GameObject nearestTarget;
        private Radar myRadar;

        private Transform myTransform;
        private Rigidbody2D myRigidBody;

        private GameObject groundCheck1, groundCheck2;

        public struct PlayerInput
        {
            public bool counterClockwise;
            public bool clockwise;
            public bool jump;
        }

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
            myRadar = GetComponentInChildren<Radar>();

            myGround = myRadar.GetMyGround();

            groundCheck1 = myTransform.Find("Ground Check 1").gameObject;
            groundCheck2 = myTransform.Find("Ground Check 2").gameObject;
        }

        [ClientCallback]
        void Update()
        {
            //Debug.LogError("Velocity:" + myRigidBody.velocity);
            //Debug.Log(syncTime / syncDelay);
            myGround = myRadar.GetMyGround();
            ApplyRotation(false);
        }

        [ClientCallback]
        void FixedUpdate()
        {
            ApplyGravity();
        }

        public void RequestMovement(PlayerInput input)
        {
            if (isLocalPlayer)
            {
                myTransform.position = Move(myTransform.position, input);
                if (input.jump)
                    Jump();
            }
        }

        private void ApplyGravity()
        {
            //float distance = Vector2.Distance(myGround.point, myTransform.position);
            Vector2 gravityVersor;
            GravityField myGravityField = myGround.collider.GetComponent<GravityField>();

            if (Vector2.Distance(myTransform.position, myGround.point) > 1 * 10f)
                gravityVersor = (myGravityField.gameObject.transform.position - myTransform.position).normalized;
            else
                gravityVersor = -myGround.normal;

            Debug.DrawRay(myTransform.position, gravityVersor, Color.red);
            GetComponent<Rigidbody2D>().AddForce(gravityVersor * myGravityField.mass);///distance);
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
            return Physics2D.OverlapArea(groundCheck1.transform.position, groundCheck2.transform.position, LayerMask.GetMask("Walkable"));
        }

        //Movement routines called by the input manager
        private Vector3 Move(Vector2 startPosition, PlayerInput input)
        {
            //if (!CanMove())
            //return myTransform.position;

            GravityField myGravityField = myGround.collider.GetComponent<GravityField>();
            RaycastHit2D platformEdge;

            Vector2 movementVersor, movementPerpendicularDown, whereGroundShouldBe, recalculatedNextPlayerPoint;

            if (input.counterClockwise)
            {
                movementVersor = new Vector3(-myGround.normal.y, myGround.normal.x);
                movementPerpendicularDown = -myGround.normal;//new Vector2(-movementVersor.y, movementVersor.x).normalized;
            }
            else if (input.clockwise)
            {
                movementVersor = new Vector3(myGround.normal.y, -myGround.normal.x);
                movementPerpendicularDown = -myGround.normal;// new Vector2(movementVersor.y, -movementVersor.x).normalized;
            }
            else
                return myTransform.position;

            Vector2 nextPlayerPoint = new Vector2(startPosition.x, startPosition.y) + movementVersor * speed * 0.2f;
            Vector2 myPosition = new Vector2(startPosition.x, startPosition.y);
            Vector2 BackRaycastDirection = -movementVersor;//(myGravityField.transform.position - myTransform.position).normalized;

            //Edge detection code

            //Casts a ray with the direction of the antinormal of the playne starting from the next predicted player position to see if there will be ground
            RaycastHit2D nextGroundCheck = Physics2D.Raycast(nextPlayerPoint, movementPerpendicularDown,
                                                               1 * EdgeCheckMultiplier,
                                                               LayerMask.GetMask("Walkable"));

            if (nextGroundCheck.collider == null && IsGrounded())//Edge detected: we obtain the next position on the platform that is grounded
            {
                /*Little Raycast scheme! P is player, arrows are raycasts, # is platform, N next player position corrected by the edge detection algorithm
                P--------->              N-P  the new corrected direction. Debug.DrawLine should help to understand what is going on =)
                ####<->N--|
                ####
                */
                whereGroundShouldBe = nextPlayerPoint + movementPerpendicularDown * 1 * EdgeCheckMultiplier;
                platformEdge = Physics2D.Raycast(whereGroundShouldBe, BackRaycastDirection, Mathf.Infinity, LayerMask.GetMask("Walkable"));
                if (platformEdge.collider != null && platformEdge.collider.gameObject.Equals(myGravityField.gameObject))
                {
                    //Debug.Log("Myland!");
                    recalculatedNextPlayerPoint = platformEdge.point + platformEdge.normal * 1;
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
                GetComponent<Rigidbody2D>().AddForce(myGround.normal * jumpPower * Time.fixedDeltaTime);
        }

        public CapsuleCollider2D GetCharacterCapsuleCollider2D()
        {
            return GetComponent<CapsuleCollider2D>();
        }

        private bool CanMove()
        {
            return true;//Insert other booleans in && for other situations in which the player cannot move
        }
    }
}
