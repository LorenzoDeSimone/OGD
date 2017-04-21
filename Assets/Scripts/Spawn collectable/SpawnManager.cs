using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public Transform PlatformsFolder;

    public GameObject CollectablePrefab;
    public GameObject CollectablePrefabBig;

    public float dropSpeed;

    private List<Transform> platforms = new List<Transform>();
    private List<PlatformSpawner> platformSpawner = new List<PlatformSpawner>();
    private List<PlatformFixedSpawner> platformFixedSpawner = new List<PlatformFixedSpawner>();
    private List<GameObject> collectables = new List<GameObject>();
    private List<GameObject> collectablesBig = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        foreach (Transform tr in PlatformsFolder)
        {
            platforms.Add(tr);
            if (tr.gameObject.GetComponent<PlatformSpawner>())
                platformSpawner.Add(tr.gameObject.GetComponent<PlatformSpawner>());
            if (tr.gameObject.GetComponent<PlatformFixedSpawner>())
                platformFixedSpawner.Add(tr.gameObject.GetComponent<PlatformFixedSpawner>());
        }
        for (int i = 0; i < 10; i++)
        {
            collectables.Add(Instantiate(CollectablePrefab, transform));
            collectables[i].SetActive(false);
            collectablesBig.Add(Instantiate(CollectablePrefabBig, transform));
            collectablesBig[i].SetActive(false);
        }

        StartCoroutine(rainOfCollectibles(10));    // RAIN OF 10 COLLECTABLE
        platformSpawner[Random.Range(0, platformSpawner.Count)].dropCollectables();             // DROP FROM A PLATFORM
        platformFixedSpawner[Random.Range(0, platformSpawner.Count)].dropCollectables();             // DROP FROM A PLATFORM WITH FIXED POSITION
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
            collectables[chosen].transform.position = position;
            collectables[chosen].SetActive(true);
        }
        else
            collectables.Add(Instantiate(CollectablePrefab, position, CollectablePrefab.transform.rotation, transform));
    }
}