using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectable : MonoBehaviour
{
    public Transform PlatformsFolder;
    public Transform PlayersFolder;

    public GameObject CollectablePrefab;
    public GameObject CollectablePrefabBig;

    public float dropSpeed;

    private List<Transform> platforms = new List<Transform>();
    private List<Transform> players = new List<Transform>();
    private List<GameObject> collectables = new List<GameObject>();
    private List<GameObject> collectablesBig = new List<GameObject>();
    private List<float> distanceFromPlayers = new List<float>();
    private float nextCollectableTime;

    // Use this for initialization
    void Start()
    {
        foreach (Transform tr in PlatformsFolder)
        {
            platforms.Add(tr);
            distanceFromPlayers.Add(0);
        }
        foreach (Transform tr in PlayersFolder)
        {
            players.Add(tr);
        }
        for (int i = 0; i < 10; i++)
        {
            collectables.Add(Instantiate(CollectablePrefab, transform));
            collectables[i].SetActive(false);
            collectablesBig.Add(Instantiate(CollectablePrefabBig, transform));
            collectablesBig[i].SetActive(false);
        }
        nextCollectableTime = Time.time + dropSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextCollectableTime)
        {
            int i, j, fate;
            float count;
            for (i = 0; i < platforms.Count; i++)
            {
                count = 0;
                foreach (Transform player in players)
                {
                    if(player.gameObject.activeSelf)
                        count += Vector3.Distance(platforms[i].position, player.position);
                }
                distanceFromPlayers[i] = count;
            }
            Transform tr;
            for (i = 0; i < platforms.Count - 1; i++)
            {
                for (j = i + 1; j < platforms.Count; j++)
                {
                    if(distanceFromPlayers[i] < distanceFromPlayers[j])
                    {
                        count = distanceFromPlayers[i];
                        distanceFromPlayers[i] = distanceFromPlayers[j];
                        distanceFromPlayers[j] = count;
                        tr = platforms[i];
                        platforms[i] = platforms[j];
                        platforms[j] = tr;
                    }
                }
            }
            i = (platforms.Count + 1 - platforms.Count % 2) * platforms.Count / 2;
            fate = Random.Range(0, i);
            i = platforms.Count;
            j = 0;
            while (i < fate)
            {
                j++;
                i += platforms.Count - j;
            }
            tr = platforms[j];
            count = Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector3 pos = tr.position + new Vector3((tr.gameObject.GetComponent<Collider2D>().bounds.size.x / 2 + 1) * Mathf.Cos(fate), (tr.gameObject.GetComponent<Collider2D>().bounds.size.y / 2 + 1) * Mathf.Sin(fate), 0);
            newCollectable(pos);
            nextCollectableTime = Time.time + dropSpeed;
        }
    }

    private void newCollectable(Vector3 position)
    {
        int chosen = -1;
        for (int i = 0; i < collectables.Count && chosen == -1; i++)
        {
            if (!collectables[i].activeSelf)
                chosen = i;
        }
        if (chosen >= 0)
        {
            collectables[chosen].transform.position = position;
            collectables[chosen].SetActive(true);
        }
        else
            collectables.Add(Instantiate(CollectablePrefab, position, CollectablePrefab.transform.rotation, transform));
    }
}
