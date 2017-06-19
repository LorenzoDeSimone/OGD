using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movable : NetworkBehaviour
    {
        public float speed = 1.0f;
        public float jumpPower = 100.0f;
        public float rotationSpeed = 5.0f;
        public float edgeCheckMultiplier = 1.1f;
        public float jumpControlStopWindow = 0.2f;

        public bool useUnityPhisics = false;

        private static readonly float rotationEpsilon = 0.999f;

        private RaycastHit2D myGround;
        private GameObject nearestTarget;
        private Radar myRadar;

        private Transform myTransform;
        private Rigidbody2D myRigidBody;

        private GameObject groundCheck1, groundCheck2;

        private SpriteRenderer spriteRenderer;
        private bool controlsEnabled = true;

        [SyncVar]
        public bool hit = false;
        public bool thisAgentCanJump = false;
        public bool thisAgentHasGravity = false;
        public bool flipSprite = true;

        private Vector3 myForces;
        private PlayerDataHolder playerDataHolder;

        public struct CharacterInput
        {
            public bool counterClockwise;
            public bool clockwise;
            public bool jump;
        }

        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
            myRadar = GetComponentInChildren<Radar>();

            myForces = new Vector3(0, 0, 0);

            myGround = myRadar.GetMyGround();

            playerDataHolder = GetComponent<PlayerDataHolder>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            myGround = myRadar.GetMyGround();
            ApplyRotation(false);
            if(!useUnityPhisics)
                transform.position += myForces * Time.deltaTime;
        }

        void FixedUpdate()
        {
            if(thisAgentHasGravity)
                ApplyGravity();
        }

        private void ApplyGravity()
        {
            if (!myGround || !myGround.collider)
                return;
            //float distance = Vector2.Distance(myGround.point, myTransform.position);
            Vector2 gravityVersor;
            Platform myGravityField = myGround.collider.GetComponent<Platform>();

            gravityVersor = -myGround.normal;

            Debug.DrawRay(myTransform.position, gravityVersor, Color.red);
            if(useUnityPhisics)
                GetComponent<Rigidbody2D>().AddForce(gravityVersor * myGravityField.mass);///distance);
            else if (!IsGrounded())
                ApplyForce(gravityVersor * myGravityField.mass);///distance);
            else if(controlsEnabled)
                myForces = Vector3.zero;
        }

        private void ApplyRotation(bool forceTargetRotation)
        {
            if (!myGround || !myGround.collider)
                return;

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
            if (thisAgentCanJump)
                return Physics2D.OverlapCircle(myTransform.position, GetCollider().bounds.extents.y, LayerMask.GetMask("Walkable"));
            //return Physics2D.OverlapArea(groundCheck1.transform.position, groundCheck2.transform.position, LayerMask.GetMask("Walkable"));
            else
                return true;
        }

        //Movement routines called by the input manager
        public Vector2 Move(CharacterInput input)//returns new movement versor just for checking
        {
            if (!CanMove())
                return Vector2.zero;

            Vector2 myPosition = myTransform.position;

            Platform myGravityField = myGround.collider.GetComponent<Platform>();
            RaycastHit2D platformEdge;

            Vector2 movementVersor, movementPerpendicularDown, whereGroundShouldBe, recalculatedNextMovablePoint;

            if (input.jump && thisAgentCanJump)
                Jump();

            if (input.counterClockwise)
            {
                movementVersor = new Vector3(-myGround.normal.y, myGround.normal.x);
                movementPerpendicularDown = -myGround.normal;//new Vector2(-movementVersor.y, movementVersor.x).normalized;

                if (flipSprite)
                {
                    SafeFlip(true); 
                }
            }
            else if (input.clockwise)
            {
                movementVersor = new Vector3(myGround.normal.y, -myGround.normal.x);
                movementPerpendicularDown = -myGround.normal;// new Vector2(movementVersor.y, -movementVersor.x).normalized;

                if (flipSprite)
                {
                    SafeFlip(false); 
                }
            }
            else
            {
                Debug.LogWarning("clockwise: " + input.clockwise + "|| counterclockwise: " + input.counterClockwise);
                return Vector2.zero;
            }

            Vector2 nextMovablePoint = myPosition + movementVersor * speed * speed/60f;
            Vector2 BackRaycastDirection = -movementVersor;//(myGravityField.transform.position - myTransform.position).normalized;
            float distanceToGround = Vector2.Distance(myPosition, myGround.point);
            //Edge detection code

            //Casts a ray with the direction of the antinormal of the playne starting from the next predicted player position to see if there will be ground
            RaycastHit2D nextGroundCheck = Physics2D.Raycast(nextMovablePoint, movementPerpendicularDown,
                                                               distanceToGround * edgeCheckMultiplier,
                                                               LayerMask.GetMask("Walkable"));

            Debug.DrawRay(myTransform.position, movementVersor, Color.magenta);

            if (nextGroundCheck.collider == null)//Edge detected: we obtain the next position on the platform that is grounded
            {
                /*Little Raycast scheme! P is player, arrows are raycasts, # is platform, N next player position corrected by the edge detection algorithm
                P--------->              N-P  the new corrected direction. Debug.DrawLine should help to understand what is going on =)
                ####<->N--|
                ####
                */
                whereGroundShouldBe = nextMovablePoint + movementPerpendicularDown * distanceToGround * edgeCheckMultiplier;
                platformEdge = Physics2D.Raycast(whereGroundShouldBe, BackRaycastDirection, Mathf.Infinity, LayerMask.GetMask("Walkable"));
                if (platformEdge.collider != null && platformEdge.collider.gameObject.Equals(myGravityField.gameObject))
                {
                    //Debug.Log("Myland!");
                    recalculatedNextMovablePoint = platformEdge.point + platformEdge.normal * distanceToGround;
                    movementVersor = (recalculatedNextMovablePoint - myPosition).normalized;

                    Debug.DrawLine(myTransform.position, nextMovablePoint, Color.blue);
                    Debug.DrawLine(nextMovablePoint, whereGroundShouldBe, Color.green);
                    Debug.DrawLine(whereGroundShouldBe, platformEdge.point, Color.yellow);
                    Debug.DrawLine(platformEdge.point, recalculatedNextMovablePoint, Color.magenta);
                   // Debug.LogError("W");
                }
            }

            myTransform.position = myPosition + movementVersor * speed * Time.deltaTime;
            return movementVersor;
        }

        private void SafeFlip(bool b)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = b;
            if (playerDataHolder)
                playerDataHolder.FlipRemoteSprite(b);
        }

        public void Jump()
        {
            if (IsGrounded())
            {
                if(useUnityPhisics)
                    GetComponent<Rigidbody2D>().AddForce(myGround.normal * jumpPower);
                else
                    ApplyForce(myGround.normal * jumpPower);
                controlsEnabled = false;
                StartCoroutine(JumpControlEnable());
            }
        }

        public bool CanMove()
        {
            return IsGrounded()            &&
                   myGround.collider!=null &&
                   !hit                    &&
                   controlsEnabled;//Insert other booleans in && for other situations in which the player cannot move
        }

        public Collider2D GetCollider()
        {
            return GetComponent<Collider2D>();
        }

        IEnumerator<WaitForSeconds> JumpControlEnable()
        {
            yield return new WaitForSeconds(jumpControlStopWindow);
            Debug.Log("enabling");
            controlsEnabled = true;
        }

        private void ApplyForce(Vector3 dir)
        {
            myForces += dir/50;
        }
    }
}
