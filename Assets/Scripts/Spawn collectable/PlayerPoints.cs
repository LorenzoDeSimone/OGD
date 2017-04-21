using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPoints : MonoBehaviour {
    private int points;
    private PointManager pointManager;

	// Use this for initialization
	void Start () {
        points = 0;
        pointManager = transform.parent.gameObject.GetComponent<PointManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addPoint()
    {
        points++;
        pointManager.updatePoints();
    }

    public void addPoints(int i)
    {
        points += i;
        pointManager.updatePoints();
    }

    public int point()
    {
        return points;
    }
}
