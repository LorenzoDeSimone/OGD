using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float jumpRate;

    private Rigidbody2D rb;
    private bool jumpAble;
    private float nextJump;
    private Transform childGuide;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAble = true;
        nextJump = Time.time;
        childGuide = transform.FindChild("guide");
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVerical = Input.GetAxis("Vertical");
        float playerAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        float sinPlayerAngle = Mathf.Sin(playerAngle);
        float cosPlayerAngle = Mathf.Cos(playerAngle);
        float joystickAngle = Mathf.Acos(moveHorizontal);
        if (moveVerical < 0)
            joystickAngle = -joystickAngle;
        if (moveHorizontal != 0 || moveVerical != 0)
        {
            float movement = Mathf.Cos(joystickAngle - playerAngle) * Mathf.Sqrt(moveHorizontal * moveHorizontal + moveVerical * moveVerical);
            transform.position += new Vector3(movement * cosPlayerAngle, movement * sinPlayerAngle, 0) * speed * Time.fixedDeltaTime;
            childGuide.position = transform.position + new Vector3(moveHorizontal, moveVerical, 0);
        }
        else
        {
            childGuide.position = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton0) == true && Time.time > nextJump && Physics2D.Raycast(new Vector2(transform.position.x - 2 * sinPlayerAngle, transform.position.y - 2 * cosPlayerAngle), new Vector2(-sinPlayerAngle, -cosPlayerAngle), 2))
        {
            rb.velocity = new Vector2(-jumpSpeed * sinPlayerAngle, jumpSpeed * cosPlayerAngle);
            nextJump = Time.time + jumpRate;
        }
    }
}
