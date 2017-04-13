using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MobilePlayerController : NetworkBehaviour
    {
        public float speed = 1.0f;
        public float jumpPower = 100.0f;
        public float rotationSpeed = 5.0f;

        private static readonly float rotationEpsilon = 0.999f;
        private GravityField myGravityField;
        private Rigidbody2D myRigidBody;

        private Transform myTransform;
        private RaycastHit2D myGround;

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
            //myGround = GetMyGround();
            //myGravityField = myGround.transform.gameObject.GetComponent < GravityField > ();
        }
 
        [ClientCallback]
        void Update()
        { }

        private void ApplyGravity()
        {
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * myGravityField.gravityStrength);
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

        private void RotateAccordinglyToMyGround()
        {
            /*
            MULTIPLE RAYCASTS WITH MEAN OF NORMALS APPROACH (MAYBE! NOT NEEDED)
            Vector3 behindRaycastStart = myTransform.position - transform.right, 
                    frontRaycastStart  = myTransform.position + transform.right;

            RaycastHit2D behindGround = Physics2D.Raycast(behindRaycastStart,
                                        myGravityField.transform.position - myTransform.position,
                                        Mathf.Infinity, LayerMask.GetMask("Walkable"));

            RaycastHit2D frontGround = Physics2D.Raycast(frontRaycastStart,
                                       myGravityField.transform.position - myTransform.position,
                                       Mathf.Infinity, LayerMask.GetMask("Walkable"));
                                       */
            //Vector3 meanNormal = myGround.normal;// GetMeanVector(new[] { myGround.normal, frontGround.normal });

            //Debug.Log(Vector3.Dot(transform.up, meanNormal));


            //Forward -> blue arrow in the editor
            //Normal -> Normal of current gravity field
            //We calculate the quaternion rotation that has the same forward vector of the current ground
            //but has the ground mean normal as the upward vector
            Quaternion targetRotation = Quaternion.LookRotation(myGround.transform.forward, myGround.normal);
            Debug.Log(Quaternion.Dot(transform.rotation, targetRotation));

            //If the rotation to do is very small, do nothing
            if (Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) > rotationEpsilon)
                return;
            //We interpolate to make the rotation smooth
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        [ClientCallback]
        void FixedUpdate()
        {
            myGround = GetMyGround();
            ApplyGravity();
            RotateAccordinglyToMyGround();
        }

        public void SetGravityCenter(GravityField newGravityField)
        {
            myGravityField = newGravityField;
        }

        private RaycastHit2D GetMyGround()
        {
            return Physics2D.Raycast(myTransform.position,
                                     myGravityField.transform.position - myTransform.position,
                                     Mathf.Infinity, LayerMask.GetMask("Walkable"));
        }

        public bool CanJump()
        {
            return Physics2D.Raycast(transform.position, myGravityField.transform.position - transform.position, 1.1f, LayerMask.GetMask("Walkable"));
        }
        
        public bool CanShoot()
        {
            return true;//Placeholder before rocket count implementation
        }


        //Movement routines called by the input manager
        public void MoveCounterclockwise()
        {
            Vector3 movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
            transform.position += movementVector * speed * Time.fixedDeltaTime;
            GetComponent<SpriteRenderer>().flipX = true;//TEMP WORKAROUND, Animations will be implemented
        }

        public void MoveClockwise()
        {
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
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
            myRigidBody.velocity = myGround.normal * jumpPower * Time.fixedDeltaTime;
        }
    }
}
