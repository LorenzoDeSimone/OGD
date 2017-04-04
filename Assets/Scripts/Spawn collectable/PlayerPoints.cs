using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPoints : MonoBehaviour {
    private int points;

	// Use this for initialization
	void Start () {
        points = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addPoint()
    {
        points++;
        GetComponentInChildren<Text>().text = "" + points;
    }

    public void addPoints(int i)
    {
        points += i;
    }
}
