using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformSpawner : MonoBehaviour
{
    public int Countdown = 0;
    public int NumOfCollectables = 0;
    public int NumOfCollectablesBig = 0;

    public GameObject CollectablePrefab;
    public GameObject CollectablePrefabBig;

    private List<GameObject> platforms = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        GameObject temp;
        for (int i = 0; i < NumOfCollectables; i++)
        {
            temp = Instantiate(CollectablePrefab, transform);
            temp.SetActive(false);
            platforms.Add(temp);
        }
        for (int i = 0; i < NumOfCollectablesBig; i++)
        {
            temp = Instantiate(CollectablePrefabBig, transform);
            temp.SetActive(false);
            platforms.Add(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void dropCollectables()
    {
        StartCoroutine(startCountdown());
    }

    private IEnumerator startCountdown()
    {
        int j, i = Countdown;
        float angle;
        Text countdownCounter = GetComponentInChildren<Text>();
        //wait countdown
        while (i > 0)
        {
            countdownCounter.text = "" + i;
            i--;
            yield return new WaitForSeconds(1);
        }
        countdownCounter.text = "";
        //spawn randomly the collectors
        GameObject temp;
        for (i = 0; i < platforms.Count; i++)
        {
            j = Random.Range(0, platforms.Count);
            temp = platforms[i];
            platforms[i] = platforms[j];
            platforms[j] = temp;
        }
        foreach (GameObject go in platforms)
        {
            if (!go.activeSelf)
            {
                angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                go.transform.position = transform.position + new Vector3((transform.gameObject.GetComponent<Collider2D>().bounds.size.x / 2 + 1) * Mathf.Cos(angle), (transform.gameObject.GetComponent<Collider2D>().bounds.size.y / 2 + 1) * Mathf.Sin(angle), 0);
                go.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
