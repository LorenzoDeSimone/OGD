using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpawnManager : NetworkBehaviour
{
    public GameObject CollectablePrefab;
    public GameObject CollectablePrefabBig;

    public float dropSpeed;

    private List<Transform> platforms = new List<Transform>();
    //private List<PlatformSpawner> platformSpawner = new List<PlatformSpawner>();
    private List<PlatformFixedSpawner> platformFixedSpawner = new List<PlatformFixedSpawner>();
    
    private List<GameObject> collectables = new List<GameObject>();
    private List<GameObject> collectablesBig = new List<GameObject>();
    private GameObject go;

    // Use this for initialization
    void Start()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Platform"))
        {
            platforms.Add(go.transform);
            //if (go.GetComponent<PlatformSpawner>())
            //    platformSpawner.Add(go.GetComponent<PlatformSpawner>());
            if (go.GetComponent<PlatformFixedSpawner>())
                platformFixedSpawner.Add(go.GetComponent<PlatformFixedSpawner>());
        }
        for (int i = 0; i < 10; i++)
        {
            go = Instantiate(CollectablePrefab, transform);
            NetworkServer.Spawn(go);
            collectables.Add(go);
            collectables[i].SetActive(false);
            //collectablesBig.Add(Instantiate(CollectablePrefabBig, transform));
            //collectablesBig[i].SetActive(false);
        }

        StartCoroutine(rainOfCollectibles(10));
        StartCoroutine(startGame());
        //platformSpawner[Random.Range(0, platformSpawner.Count)].dropCollectables();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator rainOfCollectibles(int number)
    {
        int i = 0;
        float angle;
        Transform tr;
        while (number > i)
        {
            tr = platforms[Random.Range(0, platforms.Count)];
            angle = Random.Range(0, 360) * Mathf.Deg2Rad;
            Vector3 pos = tr.position + new Vector3((tr.gameObject.GetComponent<Collider2D>().bounds.size.x / 2 + 1) * Mathf.Cos(angle), (tr.gameObject.GetComponent<Collider2D>().bounds.size.y / 2 + 1) * Mathf.Sin(angle), 0);
            newCollectable(pos);
            yield return new WaitForSeconds(dropSpeed);
            i++;
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
            ActivateCollectable(position, chosen);
        }
        else
        {
            go = Instantiate(CollectablePrefab, position, CollectablePrefab.transform.rotation, transform);
            collectables.Add(go);
            NetworkServer.Spawn(go);
        }
    }

    private IEnumerator startGame()
    {
        /*
        Text countdownCounter = GetComponentInChildren<Text>();
        countdownCounter.text = "3";
        yield return new WaitForSeconds(1);
        countdownCounter.text = "2";
        yield return new WaitForSeconds(1);
        countdownCounter.text = "1";
        yield return new WaitForSeconds(1);
        countdownCounter.text = "GO";
        yield return new WaitForSeconds(1);
        countdownCounter.text = "";
        */

        yield return new WaitForSeconds(2);
        
        // Activation Player Movement

        for (int i = 0; i < platformFixedSpawner.Count; i++)
        {
            ActivateSpawner(i);
        }
    }

    [ClientCallback]
    private void ActivateCollectable(Vector3 position, int chosen)
    {
        collectables[chosen].transform.position = position;
        collectables[chosen].SetActive(true);
    }

    [ClientCallback]
    private void ActivateSpawner(int i)
    {
        platformFixedSpawner[i].setEnabled(true);
    }
}