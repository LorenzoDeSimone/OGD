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

        Rigidbody2D rb;
        Transform tr;
        Vector3 movementVector;
        float moveLT;
        float moveRT;

        [ClientCallback]
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            tr = GetComponent<Transform>();
        }


        [ClientCallback]
        void Update()
        {
            moveLT = Input.GetAxis("Horizontal");
            moveRT = Input.GetAxis("Vertical");
        }

        [ClientCallback]
        void FixedUpdate()
        {
            RaycastHit2D myGround = GetMyGround();
            if (moveLT != 0 || moveRT != 0)
            {
                Vector3 movementVector;

                if (moveLT != 0)
                    //CounterClockwise
                    movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
                else
                    //Clockwise
                    movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);

                transform.position += movementVector * speed * Time.fixedDeltaTime;
            }
        }

        public void SetGravityCenter(GravityField newGravityField)
        {
            myGravityField = newGravityField;
        }

        private RaycastHit2D GetMyGround()
        {
            return Physics2D.Raycast(tr.position,
                myGravityField.transform.position - tr.position,
                Mathf.Infinity, LayerMask.GetMask("Walkable"));
        }
    }
}
