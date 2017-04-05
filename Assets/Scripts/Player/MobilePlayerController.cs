﻿using UnityEngine;
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
        bool previousCanJump;

        [ClientCallback]
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myTransform = GetComponent<Transform>();
            previousCanJump = InputManager.IsJumpButtonOnTap();
        }
 
        [ClientCallback]
        void Update()
        {
            RaycastHit2D myGround = GetMyGround();
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
            transform.up = Vector2.Lerp(transform.up, myGround.normal, Time.deltaTime * 10);

            if (InputManager.IsCounterclockwiseButtonPressed() || InputManager.IsClockwiseButtonPressed())
            {
                Vector3 movementVector;

                if (InputManager.IsCounterclockwiseButtonPressed())
                    movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
                else
                    movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);

                transform.position += movementVector * speed * Time.fixedDeltaTime;
            }
            //if (InputManager.IsJumpButtonOnTap() && CanJump())
                //Debug.Log("JUMP!");
                //myRigidBody.velocity = myGround.normal * jumpPower * Time.fixedDeltaTime;
            //if (InputManager.IsRocketButtonOnTap() && CanShoot())
              //  Debug.Log("BOOM!");
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

        public void Jump()
        {
            RaycastHit2D myGround = GetMyGround();
            GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
            transform.up = Vector2.Lerp(transform.up, myGround.normal, Time.deltaTime * 10);
            myRigidBody.velocity = myGround.normal * jumpPower * Time.fixedDeltaTime;
        }
    }
}
