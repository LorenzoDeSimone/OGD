using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MobilePlayerController : NetworkBehaviour
    {
        public float speed = 1.0f;
        public float jumpPower = 100.0f;
        public float rotationSpeed = 1000.0f;
        private GravityField myGravityField;
        private Rigidbody2D myRigidBody;
        int rot = 0;
        Transform myTransform;
        RaycastHit2D myGround;

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
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
        }

        private void Rotate()
        {
            //Forward -> blue arrow in the editor
            //Normal -> Normal of current gravity field
            //We calculate the quaternion rotation that has the same forward vector of the current ground
            //but has the ground normal as the upward vector
            Quaternion targetRotation = Quaternion.LookRotation(myGround.transform.forward,myGround.normal);
            //We interpolate to make the rotation smooth
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        [ClientCallback]
        void FixedUpdate()
        {
            myGround = GetMyGround();
            ApplyGravity();
            Rotate();
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
