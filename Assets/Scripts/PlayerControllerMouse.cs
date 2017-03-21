using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMouse : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float jumpRate;
    public float guideSpeed;
    public GravityFieldMouse myGravityField;

    private Rigidbody2D rb;
    private float nextJump;
    private Transform childGuide;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nextJump = Time.time;
        childGuide = transform.FindChild("guide");
    }

    void FixedUpdate()
    {
        float moveHorizontal = Camera.main.ScreenPointToRay(Input.mousePosition).origin.x - transform.position.x;
        float moveVerical = Camera.main.ScreenPointToRay(Input.mousePosition).origin.y - transform.position.y;
        if (moveHorizontal > 1) moveHorizontal = 1;
        else if (moveHorizontal < -1) moveHorizontal = -1;
        if (moveVerical > 1) moveVerical = 1;
        else if (moveVerical < -1) moveVerical = -1;
        float playerAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        float joystickCosAngle = Mathf.Acos(moveHorizontal);
        float joystickSinAngle = Mathf.Asin(moveVerical);
        float joystickAngle = 0;

        float moveLT = Input.GetAxis("LT");
        float moveRT = Input.GetAxis("RT");
        RaycastHit2D myGround = Physics2D.Raycast(transform.position, myGravityField.transform.position - transform.position, Mathf.Infinity, LayerMask.GetMask("Walkable"));

        if (moveHorizontal != 0 || moveVerical != 0)
        {
            if (moveVerical < 0)
                joystickCosAngle = -joystickCosAngle;
            if (moveHorizontal < 0)
                joystickSinAngle = Mathf.Abs(moveVerical) / moveVerical * 180 * Mathf.Deg2Rad - joystickSinAngle;

            if (playerAngle > 180 * Mathf.Deg2Rad) playerAngle = -360 * Mathf.Deg2Rad + playerAngle;

            if (Mathf.Abs(playerAngle) < 45 * Mathf.Deg2Rad || Mathf.Abs(playerAngle) > 135 * Mathf.Deg2Rad)
                joystickAngle = joystickCosAngle;
            else
                joystickAngle = joystickSinAngle;

            float movement = Mathf.Cos(joystickAngle - playerAngle) * Mathf.Sqrt(moveHorizontal * moveHorizontal + moveVerical * moveVerical);
            transform.position += new Vector3(movement * Mathf.Cos(playerAngle), movement * Mathf.Sin(playerAngle), 0) * speed * Time.fixedDeltaTime;
            childGuide.position += ((transform.position + new Vector3(moveHorizontal, moveVerical, 0)) - childGuide.position) * guideSpeed * Time.fixedDeltaTime;
        }
        else
        {
            childGuide.position += (transform.position - childGuide.position) * guideSpeed * Time.fixedDeltaTime;
        }
        /*if (Input.GetKeyDown(KeyCode.JoystickButton0) == true && Time.time > nextJump && Physics2D.Raycast(new Vector2(transform.position.x - 2 * sinPlayerAngle, transform.position.y - 2 * cosPlayerAngle), new Vector2(-sinPlayerAngle, -cosPlayerAngle), 2))
        {
            rb.velocity = new Vector2(-jumpSpeed * sinPlayerAngle, jumpSpeed * cosPlayerAngle);
            nextJump = Time.time + jumpRate;
        }*/
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) == true && CanJump())
            rb.velocity = myGround.normal * jumpSpeed * Time.fixedDeltaTime;

        //Debug.Log(myGravityField.transform.position);
        //Debug.Log("Mypos"+transform.position);
        //Vector2 myUnderPos = transform.position + Vector3.down * 2;
        //Debug.Log(-myGround.normal);


        //Debug.DrawLine(transform.position, myGravityField.transform.position);
        //Debug.Log(myGround.point);
        //Debug.DrawLine(transform.position, myGround.point);

        //        GetComponent<Rigidbody2D>().AddForce(Vector2.down * 100);
        GetComponent<Rigidbody2D>().AddForce(-myGround.normal * 100);
        //transform.up = Vector2.Lerp(transform.up, transform.position-myGravityField.transform.position, Time.deltaTime*10);
        transform.up = Vector2.Lerp(transform.up, myGround.normal, Time.deltaTime * 10);

        if (moveLT != 0 || moveRT != 0)
        {
            Vector3 movementVector;

            if (moveLT != 0)//CounterClockwise
                movementVector = new Vector3(-myGround.normal.y, myGround.normal.x);
            else//Clockwise
                movementVector = new Vector3(myGround.normal.y, -myGround.normal.x);

            transform.position += movementVector * speed * Time.fixedDeltaTime;
        }
    }

    public void setGravityCenter(GravityFieldMouse newGravityField)
    {
        myGravityField = newGravityField;
    }

    public bool CanJump()
    {
        return Physics2D.Raycast(transform.position, myGravityField.transform.position - transform.position, 1.1f, LayerMask.GetMask("Walkable"));
    }
}
