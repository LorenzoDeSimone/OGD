using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformFixedSpawner : MonoBehaviour
{
    public int minCountdown = 15;
    public int maxCountdown = 30;

    private bool enabled;

    private List<GameObject> collectables = new List<GameObject>();
    
    // Use this for initialization
    void Start()
    {
        enabled = false;
        foreach (Transform tr in transform)
            collectables.Add(tr.gameObject);
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
        int j, i = Random.Range(minCountdown, maxCountdown + 1);
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
        for (i = 0; i < collectables.Count; i++)
        {
            j = Random.Range(0, collectables.Count);
            temp = collectables[i];
            collectables[i] = collectables[j];
            collectables[j] = temp;
        }
        foreach(GameObject go in collectables)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        dropCollectables();
    }

    public void setEnabled(bool i)
    {
        enabled = i;
        if (enabled)
            dropCollectables();
    }
}
