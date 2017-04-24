using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointManager : MonoBehaviour
{
    public static PointManager instance = null;

    private List<Transform> players = new List<Transform>();
    private Vector3 barCenterPosition;
    private GameObject[] bar;
    private float barWidth;
    private float barHeight;
    private int[] points;

	// Use this for initialization
	void Start ()
    {
        int numPlayer = 0;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(go.GetComponent<Transform>());
            numPlayer++;
        }
        points = new int[numPlayer];
        bar = new GameObject[numPlayer];
        bar[0] = transform.GetChild(0).GetChild(0).gameObject;
        barWidth = bar[0].GetComponent<RectTransform>().sizeDelta.x / 100 * Screen.width;
        barHeight = bar[0].GetComponent<RectTransform>().sizeDelta.y / 100 * Screen.height;
        barCenterPosition = new Vector3(bar[0].transform.position.x, barHeight * 2, 0);
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = 0;
            if (i > 0)
                bar[i] = Instantiate(bar[0], bar[0].transform.parent);
            bar[i].SetActive(true);
            RectTransform rt = bar[i].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(barWidth / players.Count, barHeight);
            rt.position = new Vector3(barCenterPosition.x - barWidth / 2 + (i + 1f) * barWidth / numPlayer - barWidth / 2 / numPlayer, barCenterPosition.y, barCenterPosition.z);
            bar[i].gameObject.GetComponent<Image>().color = new PlayerColor().color[i];
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void addPoint(Transform player, int num)
    {
        for (int i = 0; i < players.Count; i++)
            if (player.Equals(players[i]))
                points[i] += num;
        updatePoints();
    }

    private void updatePoints()
    {
        float tot = 0;
        float oldDimensions = 0;
        for (int i = 0; i < players.Count; i++)
            tot += points[i];
        for (int i = 0; i < players.Count; i++)
        {
            float dim =  points[i] / tot;
            RectTransform rt = bar[i].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(dim * barWidth, barHeight);
            rt.position = new Vector3(barCenterPosition.x - barWidth / 2 + oldDimensions + dim * barWidth / 2, barCenterPosition.y, barCenterPosition.z);
            oldDimensions += dim * barWidth;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
