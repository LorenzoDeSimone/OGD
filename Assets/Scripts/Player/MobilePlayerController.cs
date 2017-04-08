using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MobilePlayerController : NetworkBehaviour
    {
        public float speed = 1.0f;
        public float jumpPower = 100.0f;
        public float jumpLockTime = 1.5f;
        public GravityField myGravityField;

        public Rigidbody2D myRigidBody;
        Transform myTransform;
        Vector3 movementVector;
        RaycastHit2D myGround;

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
            myGround = GetMyGround();
        }
 
        [ClientCallback]
        void Update()
        {
            myGround = GetMyGround();
            ApplyGravity();
            Rotate();
        }

        private void ApplyGravity()
        {
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
        }

        private void Rotate()
        {
            transform.up = Vector2.Lerp(transform.up, myGround.normal, Time.deltaTime * 10);
        }

        [ClientCallback]
        void FixedUpdate()
        {
      
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
        }

        public void MoveClockwise()
        {
            Vector3 movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);
            transform.position += movementVector * speed * Time.fixedDeltaTime;
        }

        public void Shoot()
        {
            Debug.Log("Shoot");
        }

        public void Jump()
        {
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
            transform.up = Vector2.Lerp(transform.up, myGround.normal, Time.deltaTime * 10);
            myRigidBody.velocity = myGround.normal * jumpPower * Time.fixedDeltaTime;
        }
    }
}
