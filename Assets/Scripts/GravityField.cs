using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class GravityField : MonoBehaviour {

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

        PlayerController player = collider.GetComponent<PlayerController>();
        if (player == null)
        {
            MobilePlayerController mobPlayer = collider.GetComponent<MobilePlayerController>();
            mobPlayer.SetGravityCenter(this);
        }
        else
        {
            player.SetGravityCenter(this);
        }
        //Debug.Log(transform.position);

        //collider.GetComponent<ComponentType>();

    }
}
