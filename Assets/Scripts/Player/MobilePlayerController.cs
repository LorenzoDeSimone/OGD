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

        private static readonly float rotationEpsilon = 0.999f;
        private Rigidbody2D myRigidBody;

        private RaycastHit2D myGround;
        private Transform myTransform;

        private Vector2 groundCheck1, groundCheck2;

        private HashSet<GravityField> myGravityFields;//A collection of gravity fields currently in player's collider
        private GravityField safeGravityField;//In case no GravityField is present in player's collider, this is used for attraction
               
        public enum MOVEMENT_DIRECTIONS { COUNTERCLOCKWISE, CLOCKWISE, STOP }

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();

            myGravityFields = new HashSet<GravityField>();

            myGround = GetMyGround();

            groundCheck1 = myTransform.Find("Ground Check 1").position;
            groundCheck2 = myTransform.Find("Ground Check 2").position;
        }

        [ClientCallback]
        void Update()
        {
            groundCheck1 = myTransform.Find("Ground Check 1").position;
            groundCheck2 = myTransform.Find("Ground Check 2").position;
            if (IsGrounded())
                Debug.Log("On Land!");
            myGround = GetMyGround();
            ApplyRotation();
        }

        [ClientCallback]
        void FixedUpdate()
        {
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            //float distance = Vector2.Distance(myGround.point, myTransform.position);
            Debug.DrawRay(myTransform.position, -GetMyGround().normal,Color.red);
            GravityField myGravityField = myGround.collider.GetComponent<GravityField>();
            GetComponent<Rigidbody2D>().AddForce(-GetMyGround().normal * myGravityField.mass);///distance);
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

        private void ApplyRotation()
        {
            //Forward -> blue arrow in the editor
            //Normal -> Normal of current gravity field
            //We calculate the quaternion rotation that has the same forward vector of the current ground
            //but has the ground mean normal as the upward vector
            RaycastHit2D myGround = GetMyGround();
            Quaternion targetRotation = Quaternion.LookRotation(myGround.transform.forward, myGround.normal);
            //Debug.Log(Quaternion.Dot(transform.rotation, targetRotation));

            //If the rotation to do is very small, we just apply it directly
            if (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) > rotationEpsilon)
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
                foreach (GravityField currField in myGravityFields)
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
        public void MoveCounterclockwise()
        {
            RaycastHit2D myGround = GetMyGround();
            Vector3 movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
            transform.position += movementVector * speed * Time.fixedDeltaTime;
            GetComponent<SpriteRenderer>().flipX = true;//TEMP WORKAROUND, Animations will be implemented
        }

        public void MoveClockwise()
        {
            RaycastHit2D myGround = GetMyGround();
            Vector3 movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);
            transform.position += movementVector * speed * Time.fixedDeltaTime;
            GetComponent<SpriteRenderer>().flipX = false;//TEMP WORKAROUND, Animations will be implemented
        }

        public void Shoot()
        {
            Debug.Log("BOOM!");
        }

        public void Jump()
        {
            if (IsGrounded())
            {
                RaycastHit2D myGround = GetMyGround();
                GetComponent<Rigidbody2D>().AddForce(myGround.normal * jumpPower * Time.fixedDeltaTime);
                //myRigidBody.velocity = myGround.normal * jumpPower;// * Time.fixedDeltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            GravityField newGravityField = collider.GetComponent<GravityField>();
            myGravityFields.Add(newGravityField);

            if (myGravityFields.Count == 1)
                safeGravityField = newGravityField;
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            GravityField exitGravityField = collider.GetComponent<GravityField>();
            myGravityFields.Remove(exitGravityField);

            if (myGravityFields.Count == 0)
                safeGravityField = exitGravityField;
        }

    }
}
