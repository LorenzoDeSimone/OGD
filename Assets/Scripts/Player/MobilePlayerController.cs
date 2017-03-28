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

        Rigidbody2D myRigidBody;
        Transform myTransform;
        Vector3 movementVector;

        private float moveLT, moveRT;
        private bool jumpPressed;

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
        }


        [ClientCallback]
        void Update()
        {
            InputHandling();
        }

        void InputHandling()//TODO change with screen button
        {
            moveLT = Input.GetAxis("Horizontal");
            moveRT = Input.GetAxis("Vertical");
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
        }

        [ClientCallback]
        void FixedUpdate()
        {
            RaycastHit2D myGround = GetMyGround();
       
            if (moveLT != 0 || moveRT != 0)
            {
                Vector3 movementVector;

                if (moveLT != 0)//CounterClockwise
                    movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
                else//Clockwise
                    movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);

                transform.position += movementVector * speed * Time.fixedDeltaTime;
            }
            if (jumpPressed && CanJump())
                myRigidBody.velocity = myGround.normal * jumpPower * Time.fixedDeltaTime;

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
    }
}
