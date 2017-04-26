using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PointManager : NetworkBehaviour
{
    public static PointManager instance = null;

    public GameObject pointBar;

    private List<Transform> players = new List<Transform>();
    private GameObject[] bar;
    private int[] points;

    // Use this for initialization
    void Start()
    {
        int numPlayer = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(go.GetComponent<Transform>());
            numPlayer++;
        }
        points = new int[numPlayer];
        bar = new GameObject[numPlayer];
        bar[0] = pointBar;

        RectTransform rt = bar[0].GetComponent<RectTransform>();
        Vector2 v2 = new Vector2();
        float oldDimension = 0, dim = 1 / players.Count;
        
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = 0;
            if (i > 0)
                bar[i] = Instantiate(bar[0], bar[0].transform.parent);
            bar[i].SetActive(true);
            rt = bar[i].GetComponent<RectTransform>();

            v2 = rt.anchorMin;
            v2.x = oldDimension;
            rt.anchorMin = v2;
            v2 = rt.anchorMax;
            v2.x = oldDimension + dim;
            rt.anchorMax = v2;
            oldDimension += dim;

            bar[i].gameObject.GetComponent<Image>().color = PlayerColor.color[i];
            players[i].gameObject.GetComponent<SpriteRenderer>().color = PlayerColor.color[i];
        }
    }

    // Update is called once per frame
    void Update()
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
        Vector2 v2 = new Vector2();
        float oldDimension = 0, dim = 0;
        for (int i = 0; i < players.Count; i++)
            tot += points[i];
        for (int i = 0; i < players.Count; i++)
        {
            dim = points[i] / tot;
            RectTransform rt = bar[i].GetComponent<RectTransform>();
            v2 = rt.anchorMin;
            v2.x = oldDimension;
            rt.anchorMin = v2;
            v2 = rt.anchorMax;
            v2.x = oldDimension + dim;
            rt.anchorMax = v2;
            oldDimension += dim;
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
