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

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < NumOfCollectables; i++)
            Instantiate(CollectablePrefab, transform).SetActive(false);
        for (int i = 0; i < NumOfCollectablesBig; i++)
            Instantiate(CollectablePrefabBig, transform).SetActive(false);
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
        int i = Countdown;
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
        foreach (Transform tr in transform)
        {
            if(!tr.gameObject.activeSelf)
            {
            angle = Random.Range(0, 360) * Mathf.Deg2Rad;
            tr.position = transform.position + new Vector3((transform.gameObject.GetComponent<Collider2D>().bounds.size.x / 2 + 1) * Mathf.Cos(angle), (transform.gameObject.GetComponent<Collider2D>().bounds.size.y / 2 + 1) * Mathf.Sin(angle), 0);
            tr.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
