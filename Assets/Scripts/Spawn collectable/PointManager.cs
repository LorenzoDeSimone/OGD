using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public GameObject[] bar;
    private Vector3 barPosition;

    private List<PlayerPoints> players = new List<PlayerPoints>();
    public int[] points;

	// Use this for initialization
	void Start ()
    {
		foreach (Transform tr in transform)
            if(tr.gameObject.GetComponent<PlayerPoints>())
                players.Add(tr.gameObject.GetComponent<PlayerPoints>());
        barPosition = bar[0].transform.position;
        points = new int[players.Count];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = 0;
            bar[i].SetActive(true);
            bar[i].transform.localScale = new Vector3(20f / players.Count, 1f, 0f);
            bar[i].transform.position = new Vector3(barPosition.x - 10f + (i + 1f) * 20f / players.Count - 10f / players.Count, barPosition.y,barPosition.z);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void updatePoints()
    {
        float tot = 0;
        float oldDimensions = 0;
        for (int i = 0; i < players.Count; i++)
        {
            points[i] = players[i].point();
            tot += points[i];
        }
        for (int i = 0; i < players.Count; i++)
        {
            float dim =  points[i] / tot;
            bar[i].transform.localScale = new Vector3(dim * 20f, 1f, 0f);
            bar[i].transform.position = new Vector3(barPosition.x - 10f + oldDimensions + dim * 10f, barPosition.y, barPosition.z);
            oldDimensions += dim * 20f;
        }
    }
}
