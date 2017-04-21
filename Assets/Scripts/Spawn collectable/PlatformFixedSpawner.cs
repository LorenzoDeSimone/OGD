using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformFixedSpawner : MonoBehaviour
{
    public int Countdown = 0;

    private List<GameObject> platforms = new List<GameObject>();
    
    // Use this for initialization
    void Start()
    {
        foreach (Transform tr in transform)
            platforms.Add(tr.gameObject);
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
        GameObject temp;
        int j, i = Countdown;
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
        for (i = 0; i < platforms.Count; i++)
        {
            j = Random.Range(0, platforms.Count);
            temp = platforms[i];
            platforms[i] = platforms[j];
            platforms[j] = temp;
        }
        foreach(GameObject go in platforms)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
