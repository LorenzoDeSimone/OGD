using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerCameraController : MonoBehaviour {

    public float speed = 5;
    public float zoomSpeed = 5;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.L))
        {
            transform.position = transform.position + Vector3.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.J))
        {
            transform.position = transform.position + Vector3.left * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.I))
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.K))
        {
            transform.position = transform.position + Vector3.down * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.M))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1 * Time.deltaTime * speed);
        }
        else if (Input.GetKey(KeyCode.N))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1 * Time.deltaTime * speed);
        }
    }
}
