using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBig : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            gameObject.SetActive(false);
            coll.gameObject.GetComponent<PlayerPoints>().addPoints(3);
        }
    }
}
