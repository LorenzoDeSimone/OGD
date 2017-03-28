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

        private bool counterClockwisePressed, clockwisePressed, jumpPressed;

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
            counterClockwisePressed = Input.GetKey(KeyCode.LeftArrow);
            clockwisePressed = Input.GetKey(KeyCode.RightArrow);
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
        }

        [ClientCallback]
        void FixedUpdate()
        {
            RaycastHit2D myGround = GetMyGround();
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
            transform.up = Vector2.Lerp(transform.up, myGround.normal, Time.deltaTime * 10);

            if (counterClockwisePressed || clockwisePressed)
            {
                Vector3 movementVector;

                if (counterClockwisePressed)
                    movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
                else
                    movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);

                transform.position += movementVector * speed * Time.fixedDeltaTime;
            }
            if (jumpPressed && CanJump())
            {
                myRigidBody.velocity = myGround.normal * jumpPower * Time.fixedDeltaTime;
            }
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
