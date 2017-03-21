using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if (coll.gameObject.tag.StartsWith("Robot"))
        //{

        //}

        PlayerControllerMouse player = collider.GetComponent<PlayerControllerMouse>();
        player.setGravityCenter(this);
        Debug.Log(transform.position);

        //collider.GetComponent<ComponentType>();

    }
}
